using HotelBookingAPI.Data;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;
    public AuthService(AppDbContext dbContext, ILogger<AuthService> logger, IJwtService jwtService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // if the user is not found, return null
        if (user == null)
        {
            return null;
        }

        // verify the password
        var IsValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!IsValidPassword)
        {
            return null;
        }

        // generate JWT token 
        var token = _jwtService.GenerateToken(user);
        var response = new AuthResponseDto
        {
            Token = token,
            UserId = user.UserId,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role
        };
        _logger.LogInformation("User {Email} logged in successfully.", user.Email);
        return response;
    }
    public async Task<string?> Register(RegisterDto dto)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        // if a user with the same email already exists, return an error message
        if (existingUser != null)
        {
            return "User registered with this email already exists.";
        }

        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        };

        // store the user into DB
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("User {Email} registered successfully.", user.Email);

        return $"User {user.Email} Registered successfully";
    }
}