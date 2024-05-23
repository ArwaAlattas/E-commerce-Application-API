using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using api.Controllers;
using api.Dtos;
using api.Dtos.User;
using api.Middlewares;
using api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;

[ApiController]
[Route("api/")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;
    private readonly AuthService _authService;
    private readonly ILogger<UserController> _logger;

    public UserController(UserService userService, ILogger<UserController> logger, AuthService authService)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger;
        _authService = authService;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("account/dashboard/users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        if (users == null)
        {
            throw new NotFoundException("No user Found");
        }
        return ApiResponse.Success(users, "all users are returned successfully");
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("account/dashboard/users/{userId}")]
    public IActionResult GetUser(Guid userId)
    {
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    // Singed in user only can get the information of their account
    [HttpGet("account/my-Profile")]
    public IActionResult GetUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = _userService.GetUserById(userId);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User Returned");
    }

    [HttpPost("signup")]
    public async Task<IActionResult> CreateUser(UserModel newUser)
    {
        var createdUser = await _userService.CreateUser(newUser);
        if (createdUser != null)
        {
            return ApiResponse.Created("User is created successfully");
        }
        else
        {
            throw new Exception("An error occurred while creating the user.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginUser([FromBody] LoginDto loginDto)
    {
        if (!ModelState.IsValid)
        {
            throw new BadRequestException("Invalid User Data");
        }
        var loggedInUser = await _userService.LoginUserAsync(loginDto);
        if (loggedInUser == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _authService.GenerateJwt(loggedInUser);
        return ApiResponse.Success(new { token, loggedInUser }, "User Logged In successfully");

    }
     [Authorize]
    [HttpPut("users/{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUser)
    {
        // var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        // if (string.IsNullOrEmpty(userIdString))
        // {
        //     throw new UnauthorizedAccessException("User Id is missing from token");
        // }
        // if (!Guid.TryParse(userIdString, out Guid userId))
        // {
        //     throw new BadRequestException("Invalid User Id");
        // }
        var user = await _userService.UpdateUser(userId, updateUser);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User is updated successfully");
    }

    [HttpPut("account/my-profile/update")]
    public async Task<IActionResult> UpdateUser(UpdateUserDto updateUser)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var user = await _userService.UpdateUser(userId, updateUser);
        if (user == null)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Success(user, "User is updated successfully");
    }


    [HttpDelete("account/my-profile/delete")]
    public async Task<IActionResult> DeleteUser()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString))
        {
            throw new UnauthorizedAccessException("User Id is missing from token");
        }
        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            throw new BadRequestException("Invalid User Id");
        }
        var result = await _userService.DeleteUser(userId);
        if (!result)
        {
            throw new NotFoundException("User does not exist or an invalid Id is provided");
        }
        return ApiResponse.Deleted("User is deleted successfully");
    }
}
