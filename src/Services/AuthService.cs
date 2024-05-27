using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Dtos;
using Microsoft.IdentityModel.Tokens;

namespace api.Services
{
    public class AuthService
    {

        public AuthService()
        {
            Console.WriteLine($"Jwt__Key: {Environment.GetEnvironmentVariable("Jwt__Key")}");
        }

        public string GenerateJwt(UserDto user)
        {

            var jwtKey = Environment.GetEnvironmentVariable("Jwt__Key") ?? throw new InvalidOperationException("JWT Key is missing in environment variables.");
            var jwtIssuer = Environment.GetEnvironmentVariable("Jwt__Issuer") ?? throw new InvalidOperationException("JWT Issuer is missing in environment variables.");
            var jwtAudience = Environment.GetEnvironmentVariable("Jwt__Audience") ?? throw new InvalidOperationException("JWT Audience is missing in environment variables.");
            var key = Encoding.ASCII.GetBytes(jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                new Claim(ClaimTypes.Role, user.IsAdmin? "Admin" : "User"),
                new Claim(ClaimTypes.Role, user.IsBanned? "Banned" : "notBanned"),
            }),

                Expires = DateTime.UtcNow.AddHours(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),

                Issuer = jwtIssuer,
                Audience = jwtAudience,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}