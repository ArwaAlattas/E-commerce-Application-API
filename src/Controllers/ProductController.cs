using System.ComponentModel.DataAnnotations;
using api.Controllers;
using api.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("/api/")]
public class ProductController : ControllerBase
{

    private readonly ProductService _productService;
    public ProductController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("products")]
    public async Task<IActionResult> GetAllProduct([FromQuery]List<Guid>? SelectedCategories,string? keyword,decimal? minPrice, decimal? maxPrice, string? sortBy, bool isAscending,[FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        var product = await _productService.GetAllProductService(SelectedCategories,pageNumber, pageSize,keyword, sortBy, isAscending,minPrice, maxPrice);
        if (product == null)
        {
            throw new NotFoundException("No Product Found");
        }
        return ApiResponse.Success(product, "All products are returned successfully");
    }

    [HttpGet("products/search")]
    public async Task<IActionResult> SearchProducts(string? keyword, decimal? minPrice, decimal? maxPrice, string? sortBy, bool isAscending, int page = 1,int pageSize = 3)
    {
        var products = await _productService.SearchProductsAsync(keyword, minPrice, maxPrice, sortBy, isAscending, page, pageSize);
        if (products != null)
        {
              return ApiResponse.Success(products, "All products are returned successfully");
        }
        else
        {
            throw new NotFoundException("No products found matching the search keyword");
        }
    }

    [HttpGet("products/{productId}")]
    public async Task<IActionResult> GetProductById(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var product = await _productService.GetProductById(productIdGuid);
        if (product == null)
        {
            throw new NotFoundException("No Product Found");
        }
        else
        {
            return ApiResponse.Success(product, "single product is returned successfully");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("products")]
    public async Task<IActionResult> AddProduct(ProductModel newProduct)
    {
        var response = await _productService.AddProductAsync(newProduct);
        return ApiResponse.Success(response,"Product is created successfully");;
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("products/{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, ProductModel updateProduct)
    {
        var result = await _productService.UpdateProductService(productId, updateProduct);
       if (result== null)
        {
            throw new NotFoundException("product Not Found");
        }
        return ApiResponse.Success(result,"Product is Updated successfully");
            }

    [Authorize(Roles = "Admin")]
    [HttpDelete("products/{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(string productId)
    {
        if (!Guid.TryParse(productId, out Guid productIdGuid))
        {
            throw new BadRequestException("Invalid product ID format");
        }
        var result = await _productService.DeleteProductService(productIdGuid);
        if (result== null)
        {
            throw new NotFoundException("No Product Found");
        }
        return ApiResponse.Success(result,"product is Deleted successfully");
    }
}