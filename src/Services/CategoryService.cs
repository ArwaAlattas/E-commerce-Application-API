using Dtos.Pagination;
using Microsoft.EntityFrameworkCore;

public class CategoryService
{
    private AppDBContext _appDbContext;
    public CategoryService(AppDBContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<PaginationResult<Category>> GetAllCategoryService(int pageNumber, int pageSize, string? searchKeyword, string? sortBy = null, bool isAscending = true)
    {
        var query = _appDbContext.Categories.Include(c => c.Products).AsQueryable();

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(c => c.Name.ToLower().Contains(searchKeyword.ToLower()));
        }
        if (!string.IsNullOrEmpty(sortBy.ToLower()))
        {
            query = isAscending ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
        }
        else
        {
            query = query.OrderBy(c => c.CreatedAt);
        }

         var categories = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

             var totalCount = query.Count();
        return new PaginationResult<Category>
        {
            Items = categories,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

    }

    public async Task<Category?> GetCategoryById(Guid categoryId)
    {
        return await _appDbContext.Categories.Include(c => c.Products).FirstOrDefaultAsync(c => c.CategoryID == categoryId);
    }

    public async Task<bool> CreateCategoryService(CategoryModel newCategory)
    {
        Category category = new Category
        {
            Name = newCategory.Name,
            Description = newCategory.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _appDbContext.Categories.AddAsync(category);
        await _appDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateCategoryService(Guid categoryId, CategoryModel updateCategory)
    {
        var existingCategory = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (existingCategory != null)
        {
            existingCategory.Name = updateCategory.Name;
            existingCategory.Description = updateCategory.Description;
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;

    }

    public async Task<bool> DeleteCategoryService(Guid categoryId)
    {
        var categoryToRemove = await _appDbContext.Categories.FirstOrDefaultAsync(c => c.CategoryID == categoryId);
        if (categoryToRemove != null)
        {
            _appDbContext.Categories.Remove(categoryToRemove);
            await _appDbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }
}