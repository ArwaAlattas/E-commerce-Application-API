using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework;

[Table("Products")]
public class Product
{
    public Guid ProductID { get; set; } = Guid.NewGuid();
  [Required(ErrorMessage = "Product image is required")]
  [MaxLength(255)]
    public string? ImgUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Product name is required")]
    [StringLength(50)]
    public string ProductName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Quantity is required")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "Price is required")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "CategoryId is required")]
    public Guid CategoryId { get; set; }

    public virtual Category? Category { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public Guid? CartId { get; set; } 

    public virtual Cart? Cart {set; get;} 
    public List<Order> Orders { get; set; } = new List<Order>();
}
