using HotelBookingWebsite.Enum;

public class AuthResponseDto
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public string FullName { get; set; }    
    public string Email { get; set; }
    public UserRole Role { get; set; }
}