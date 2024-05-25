using api.Dtos;
using api.Dtos.User;
using AutoMapper;
using Dtos.Pagination;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDBContext _dbContext;
    private readonly ILogger<UserService> _logger;
    private readonly IMapper _mapper;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(AppDBContext dbContext, ILogger<UserService> logger, IPasswordHasher<User> passwordHasher, IMapper mapper)
    {

        _passwordHasher = passwordHasher;
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<PaginationResult<UserDto>> GetAllUsersAsync(int pageNumber, int pageSize, string? searchKeyword, string? sortBy = null, bool isAscending = true)
    {
        var query = _dbContext.Users.Where(u => !u.IsAdmin);

        if (!string.IsNullOrEmpty(searchKeyword))
        {
            query = query.Where(u => u.Username.ToLower().Contains(searchKeyword.ToLower()));
        }

        if (!string.IsNullOrEmpty(sortBy))
        {
            switch (sortBy.ToLower())
            {
                case "date":
                    query = isAscending ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt);
                    break;
                default:
                    query = isAscending ? query.OrderBy(u => u.Username) : query.OrderByDescending(u => u.Username);
                    break;
            }
        }
        else
        {
            query = query.OrderBy(u => u.CreatedAt);
        }

      
        var users = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var totalCount = await query.CountAsync(); 

       
        var userDtos = users.Select(user => _mapper.Map<UserDto>(user)).ToList();

        return new PaginationResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };

    }

    public async Task<UserDto?> GetUserById(Guid userId)
    {
        // return await _dbContext.Users.Include(u => u.Orders).FirstOrDefaultAsync(u => u.UserID == userId);
        var user = await _dbContext.Users.FindAsync(userId);
        var userDto = _mapper.Map<UserDto>(user);
        return userDto;
    }

    public async Task<User> CreateUser(UserModel newUser)
    {
        User createUser = new User
        {
            Username = newUser.Username,
            Email = newUser.Email,
            Password = _passwordHasher.HashPassword(null, newUser.Password),
            FirstName = newUser.FirstName,
            ImgUrl = newUser.ImgUrl,
            LastName = newUser.LastName,
            PhoneNumber = newUser.PhoneNumber,
            CreatedAt = DateTime.UtcNow,
            BirthDate = newUser.BirthDate,
            Address = newUser.Address,
            IsAdmin = newUser.IsAdmin,
            IsBanned = newUser.IsBanned
        };

        _dbContext.Users.Add(createUser);

        await _dbContext.SaveChangesAsync();

        return createUser;
    }

    public async Task<User> UpdateUser(Guid userId, UpdateUserDto updateUser)
    {
        var existingUser = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (existingUser != null && updateUser != null)
        {
            existingUser.Username = updateUser.Username ?? existingUser.Username;
            existingUser.FirstName = updateUser.FirstName ?? existingUser.FirstName;
            existingUser.ImgUrl = updateUser.ImgUrl ?? existingUser.ImgUrl;
            existingUser.LastName = updateUser.LastName ?? existingUser.LastName;
            existingUser.Address = updateUser.Address ?? existingUser.Address;
            existingUser.PhoneNumber = updateUser.PhoneNumber ?? existingUser.PhoneNumber;
            existingUser.BirthDate = updateUser.BirthDate ?? existingUser.BirthDate;
            if (existingUser.IsAdmin == updateUser.IsAdmin)
            {
                existingUser.IsAdmin = existingUser.IsAdmin;
            }
            else if (!updateUser.IsAdmin.HasValue)
            {
                existingUser.IsAdmin = existingUser.IsAdmin;
            }
            else
            {
                existingUser.IsAdmin = updateUser.IsAdmin.Value;
            }
            if (existingUser.IsBanned == updateUser.IsBanned)
            {
                existingUser.IsBanned = existingUser.IsBanned;
            }
            else if (!updateUser.IsBanned.HasValue)
            {
                existingUser.IsBanned = existingUser.IsBanned;
            }
            else
            {
                existingUser.IsBanned = updateUser.IsBanned.Value;
            }
            await _dbContext.SaveChangesAsync();

            return existingUser; // Return true indicating successful update
        }

        return null; // Return false if either existingUser or updateUser is null
    }
     
      public async Task<User> BanUnbanUser(Guid userId)
    {

        var userToDelete = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (userToDelete != null)
        {
            userToDelete.IsBanned = !userToDelete.IsBanned;
            await _dbContext.SaveChangesAsync();
            return userToDelete;
        }
        return null;
    }
    public async Task<User> DeleteUser(Guid userId)
    {

        var userToDelete = _dbContext.Users.FirstOrDefault(u => u.UserID == userId);
        if (userToDelete != null)
        {
            _dbContext.Users.Remove(userToDelete);
            await _dbContext.SaveChangesAsync();
            return userToDelete;
        }
        return null;
    }

    public async Task<UserDto?> LoginUserAsync(LoginDto loginDto)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == loginDto.Email);
        if (user == null)
        {
            return null;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.Password, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
        {
            return null;
        }
        var userDto = new UserDto
        {
            UserID = user.UserID,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            ImgUrl = user.ImgUrl,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            Address = user.Address,
            IsAdmin = user.IsAdmin,
            IsBanned = user.IsBanned
        };

        return userDto;
    }

}
