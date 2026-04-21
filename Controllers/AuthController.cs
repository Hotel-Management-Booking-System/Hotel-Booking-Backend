using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var response = await _authService.Login(dto);

        // if null invalid credentials 
        if (response is null)
        {
            return BadRequest("Invalid credentials");
        }

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<ActionResult<string>> Register(RegisterDto dto)
    {
        var response = await _authService.Register(dto);

        // if null then user already exists
        if (response is null)
        {
            return BadRequest("User already exists");
        }

        return Ok(new { message = response });
    }
}