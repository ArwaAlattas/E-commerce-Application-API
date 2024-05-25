using AutoMapper;
using Dtos.Pagination;
using Microsoft.EntityFrameworkCore;
public class ProductService
{
    private readonly AppDBContext _appDbContext;
    private readonly IMapper _mapper;
    public ProductService(AppDBContext appDBContext, IMapper mapper)
    {
        _appDbContext = appDBContext;
        _mapper = mapper;
    }

    public static string GenerateSlug(string name)
    {
        return name.ToLower().Replace(" ", "-");
    }
    public async Task<PaginationResult<Product>> GetAllProductService(List<Guid>? SelectedCategories,int pageNumber, int pageSize,string? searchKeyword, string? sortBy = null, bool isAscending = true,decimal? minPrice = 0, decimal? maxPrice = decimal.MaxValue)//public List<Guid>? SelectedCategories ( get; set; } = new List<Guid>();
    {
        var query =  _appDbContext.Products
            .Include(p => p.Category).AsQueryable();

      

        if(!string.IsNullOrEmpty(searchKeyword)){
            query = query.Where(p => p.ProductName
        .ToLower().Contains(searchKeyword.ToLower())) ;
         }


        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "price":
                    query = isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "date":
                    query = query = isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = isAscending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(p => p.CreatedAt);
        }

     if (SelectedCategories != null && SelectedCategories.Any()){
        //   query = query.Where(p => p.Categories.Any(c => SelectedCategories.Contains(c.Categoryld)));
          query = query.Where(p => SelectedCategories.Contains(p.CategoryId)); 
        }
        if (minPrice > 0)
        {
            query = query.Where(p => p.Price >= minPrice);
        }

        if (maxPrice < decimal.MaxValue)
        {
            query = query.Where(p => p.Price <= maxPrice);
        }

   var products = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

            var totalCount = query.Count();
        return new PaginationResult<Product>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<Product?> GetProductById(Guid productId)
    {
        return await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductID == productId); //Include(p => p.Category).
    }

    public async Task<Product> AddProductAsync(ProductModel newProduct)
    {
        Product product = new Product
        {

            ProductID = Guid.NewGuid(),
            ProductName = newProduct.ProductName,
            ImgUrl = newProduct.ImgUrl,
            Description = newProduct.Description,
            Slug = GenerateSlug(newProduct.ProductName),
            Quantity = newProduct.Quantity,
            Price = newProduct.Price,
            CategoryId = newProduct.CategoryID,
            CreatedAt = DateTime.UtcNow

        };
        await _appDbContext.Products.AddAsync(product);
        await _appDbContext.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductService(Guid productId, ProductModel updateProduct)
    {
        var existingProduct = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (existingProduct != null)
        {
            existingProduct.ProductName = updateProduct.ProductName ?? existingProduct.ProductName;
            existingProduct.ImgUrl = updateProduct.ImgUrl??existingProduct.ImgUrl;
            existingProduct.Description = updateProduct.Description??existingProduct.Description;
            existingProduct.Slug = updateProduct.Slug ?? existingProduct.Slug ;
            existingProduct.Quantity = updateProduct.Quantity ;
            existingProduct.Price = updateProduct.Price;
            existingProduct.CategoryId = updateProduct.CategoryID;
            await _appDbContext.SaveChangesAsync();
            return existingProduct;
        }
        return null;
    }

    public async Task<Product> DeleteProductService(Guid productId)
    {
        var productToRemove = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (productToRemove != null)
        {
            _appDbContext.Products.Remove(productToRemove);
            await _appDbContext.SaveChangesAsync();
            return productToRemove;
        }
        return null;
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string? searchKeyword, decimal? minPrice = 0, decimal? maxPrice = decimal.MaxValue, string? sortBy = null, bool isAscending = true, int page = 1, int pageSize = 3)
    {
        var query = _appDbContext.Products
        .Where(p => p.ProductName
        .ToLower().Contains(searchKeyword.ToLower())); //called on the product name and the search keyword so they can get matched


        if (minPrice > 0)
        {
            query = query.Where(p => p.Price >= minPrice);
        }

        if (maxPrice < decimal.MaxValue)
        {
            query = query.Where(p => p.Price <= maxPrice);
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "price":
                    query = isAscending ? query.OrderBy(p => p.Price) : query.OrderByDescending(p => p.Price);
                    break;
                case "date":
                    query = query = isAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt);
                    break;
                default:
                    query = isAscending ? query.OrderBy(p => p.ProductName) : query.OrderByDescending(p => p.ProductName);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(p => p.CreatedAt);
        }

        // Pagination
        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return products;
    }
}