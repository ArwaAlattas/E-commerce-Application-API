using System.ComponentModel.DataAnnotations;

public class UserModel 
{
    public Guid UserID { get; set; } 

    [Required(ErrorMessage = "Username is required")]
    public required string Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    public required string Email { get; set; }


    [Required(ErrorMessage = "Password is required")]
    public required string Password { get; set; }

    [Required(ErrorMessage = "First name is required")]
    
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Last name is required")]

    public required string LastName { get; set; } 

    public string PhoneNumber { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false; 
    public bool IsBanned { get; set; } = false; 
    public DateTime BirthDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

    public List<CustomerOrderModel> Orders { get; set; } = new List<CustomerOrderModel>();

    public List<CartModel> Carts { get; set; } = new List<CartModel>();
}
