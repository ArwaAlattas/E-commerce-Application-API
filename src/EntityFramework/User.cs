using System.ComponentModel.DataAnnotations.Schema;
using EntityFramework;


[Table("Users")]
public class User
{
    public Guid UserID { get; set; } 

    public required string Username { get; set; }

    public string ImgUrl { get; set; } = string.Empty;

    public required string Email { get; set; }

    public required string Password { get; set; }

    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string PhoneNumber { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } 
    public bool IsBanned { get; set; } 
    public DateTime? BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public List<Order> Orders { get; set; } = new List<Order>();

    public List<Cart> Carts { get; set; } = new List<Cart>();
}
