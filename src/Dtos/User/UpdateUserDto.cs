namespace api.Dtos
{
    public class UpdateUserDto
    {
    public string? Username { get; set; }
    
    public string? ImgUrl { get; set; } 

    public  string? FirstName { get; set; }

    public string? LastName { get; set; } 

    public string? PhoneNumber { get; set; }
    
    public string? Address { get; set; } 
    public bool? IsAdmin { get; set; } 
     public bool? IsBanned { get; set; } 
    public DateTime? BirthDate { get; set; }
    }
}