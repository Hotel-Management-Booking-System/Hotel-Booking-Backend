using HotelBookingAPI.Data;
using HotelBookingAPI.Interfaces;
using Microsoft.EntityFrameworkCore;

public class AuthService : IAuthService
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthService> _logger;
    private readonly IJwtService _jwtService;
    private readonly IEmailService _emailService;

    public AuthService(AppDbContext dbContext, ILogger<AuthService> logger, IJwtService jwtService, IEmailService emailService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _jwtService = jwtService;
        _emailService = emailService;
    }

    public async Task<AuthResponseDto?> Login(LoginDto dto)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null)
        {
            return null;
        }

        var IsValidPassword = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
        if (!IsValidPassword)
        {
            return null;
        }

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

        // Send login notification email
        await _emailService.SendLoginNotificationEmailAsync(user.Email, user.FullName);

        return response;
    }

    public async Task<string?> Register(RegisterDto dto)
    {
        var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

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

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation("User {Email} registered successfully.", user.Email);

        // Send welcome email
        await _emailService.SendWelcomeEmailAsync(user.Email, user.FullName);

        return $"User {user.Email} Registered successfully";
    }
}