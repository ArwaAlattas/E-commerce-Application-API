using AutoMapper;
using Dtos.Pagination;
using Microsoft.EntityFrameworkCore;
public class ProductService
{
    private readonly AppDBContext _appDbContext;
     private readonly IMapper _mapper;
    public ProductService(AppDBContext appDBContext,IMapper mapper)
    {
        _appDbContext = appDBContext;
         _mapper = mapper;
    }

    public static string GenerateSlug(string name){
      return  name.ToLower().Replace(" ","-");
      }
    public async Task<PaginationResult<ProductModel>> GetAllProductService(int pageNumber, int pageSize)
    {
        var totalCount = _appDbContext.Products.Count();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);
        var page = await _appDbContext.Products
            .OrderByDescending(b => b.CreatedAt)
            .ThenByDescending(b => b.ProductID)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(product => _mapper.Map<ProductModel>(product)).ToListAsync();
            // .Include(p => p.Category)
            // .ToListAsync();

        return new PaginationResult<ProductModel>
        {
            Items = page,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
    }

    public async Task<Product?> GetProductById(Guid productId)
    {
        return await _appDbContext.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.ProductID == productId);
    }

    public async Task<Guid> AddProductAsync(ProductModel newProduct)
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
        return product.ProductID;
    }

    public async Task<bool> UpdateProductService(Guid productId, ProductModel updateProduct)
    {
        var existingProduct = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (existingProduct != null)
        {
            existingProduct.ProductName = updateProduct.ProductName;
            existingProduct.ImgUrl = updateProduct.ImgUrl;
            existingProduct.Description = updateProduct.Description;
            existingProduct.Slug = updateProduct.Slug;
            existingProduct.Quantity = updateProduct.Quantity;
            existingProduct.Price = updateProduct.Price;
            existingProduct.CategoryId = updateProduct.CategoryID;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<bool> DeleteProductService(Guid productId)
    {
        var productToRemove = await _appDbContext.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (productToRemove != null)
        {
            _appDbContext.Products.Remove(productToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
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