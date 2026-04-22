using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Interfaces
{
    public interface IEmailService
    {
        Task SendWelcomeEmailAsync(string toEmail, string fullName);
        Task SendLoginNotificationEmailAsync(string toEmail, string fullName);
        Task SendBookingConfirmationEmailAsync(string toEmail, string fullName, BookingConfirmationEmailDto dto);
    }
}