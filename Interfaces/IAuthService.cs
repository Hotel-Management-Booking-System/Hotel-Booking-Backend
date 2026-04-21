public interface IAuthService
{
    Task<AuthResponseDto?> Login(LoginDto dto);
    Task<string?> Register(RegisterDto dto);
}