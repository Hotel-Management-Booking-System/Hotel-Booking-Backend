using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace HotelBookingAPI.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        private async Task SendEmailAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.AppPassword);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string fullName)
        {
            var subject = "Welcome to Hotel Booking System!";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e0e0e0;border-radius:8px;overflow:hidden;'>
                    <div style='background-color:#1a73e8;padding:24px;text-align:center;'>
                        <h1 style='color:white;margin:0;'>Hotel Booking System</h1>
                    </div>
                    <div style='padding:32px;'>
                        <h2 style='color:#333;'>Welcome, {fullName}! 🎉</h2>
                        <p style='color:#555;font-size:15px;line-height:1.6;'>
                            Thank you for registering with us. Your account has been created successfully.
                        </p>
                        <p style='color:#555;font-size:15px;line-height:1.6;'>
                            You can now browse hotels, check room availability, and make bookings with ease.
                        </p>
                        <div style='background-color:#f5f5f5;border-radius:6px;padding:16px;margin-top:24px;'>
                            <p style='margin:0;color:#333;font-size:14px;'>
                                If you did not create this account, please contact our support team immediately.
                            </p>
                        </div>
                    </div>
                    <div style='background-color:#f9f9f9;padding:16px;text-align:center;border-top:1px solid #e0e0e0;'>
                        <p style='color:#999;font-size:12px;margin:0;'>© 2024 Hotel Booking System. All rights reserved.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, fullName, subject, body);
        }

        public async Task SendLoginNotificationEmailAsync(string toEmail, string fullName)
        {
            var subject = "New Login Detected - Hotel Booking System";
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e0e0e0;border-radius:8px;overflow:hidden;'>
                    <div style='background-color:#1a73e8;padding:24px;text-align:center;'>
                        <h1 style='color:white;margin:0;'>Hotel Booking System</h1>
                    </div>
                    <div style='padding:32px;'>
                        <h2 style='color:#333;'>Hello, {fullName} 👋</h2>
                        <p style='color:#555;font-size:15px;line-height:1.6;'>
                            We noticed a new login to your account on <strong>{DateTime.UtcNow:dddd, dd MMM yyyy} at {DateTime.UtcNow:HH:mm} UTC</strong>.
                        </p>
                        <div style='background-color:#fff8e1;border-left:4px solid #f9a825;padding:16px;border-radius:4px;margin-top:16px;'>
                            <p style='margin:0;color:#555;font-size:14px;'>
                                If this was you, no action is needed. If you did not log in, please reset your password immediately.
                            </p>
                        </div>
                    </div>
                    <div style='background-color:#f9f9f9;padding:16px;text-align:center;border-top:1px solid #e0e0e0;'>
                        <p style='color:#999;font-size:12px;margin:0;'>© 2024 Hotel Booking System. All rights reserved.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, fullName, subject, body);
        }

        public async Task SendBookingConfirmationEmailAsync(string toEmail, string fullName, BookingConfirmationEmailDto dto)
        {
            var subject = "Booking Confirmed - Hotel Booking System";
            int totalNights = (dto.CheckOutDate - dto.CheckInDate).Days;
            var body = $@"
                <div style='font-family:Arial,sans-serif;max-width:600px;margin:auto;border:1px solid #e0e0e0;border-radius:8px;overflow:hidden;'>
                    <div style='background-color:#1a73e8;padding:24px;text-align:center;'>
                        <h1 style='color:white;margin:0;'>Hotel Booking System</h1>
                    </div>
                    <div style='padding:32px;'>
                        <h2 style='color:#333;'>Booking Confirmed! 🏨</h2>
                        <p style='color:#555;font-size:15px;line-height:1.6;'>
                            Dear <strong>{fullName}</strong>, your booking has been confirmed successfully. Here are your booking details:
                        </p>
                        <table style='width:100%;border-collapse:collapse;margin-top:20px;'>
                            <tr style='background-color:#f5f5f5;'>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;width:40%;'>Hotel</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{dto.HotelName}</td>
                            </tr>
                            <tr>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Room Type</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{dto.RoomType}</td>
                            </tr>
                            <tr style='background-color:#f5f5f5;'>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Check-In</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{dto.CheckInDate:dddd, dd MMM yyyy}</td>
                            </tr>
                            <tr>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Check-Out</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{dto.CheckOutDate:dddd, dd MMM yyyy}</td>
                            </tr>
                            <tr style='background-color:#f5f5f5;'>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Total Nights</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{totalNights} night(s)</td>
                            </tr>
                            <tr>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Guests</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#555;'>{dto.NumberOfGuests}</td>
                            </tr>
                            <tr style='background-color:#e8f5e9;'>
                                <td style='padding:12px;border:1px solid #e0e0e0;font-weight:bold;color:#333;'>Total Price</td>
                                <td style='padding:12px;border:1px solid #e0e0e0;color:#2e7d32;font-weight:bold;font-size:16px;'>₹{dto.TotalPrice:F2}</td>
                            </tr>
                        </table>
                        <p style='color:#555;font-size:14px;margin-top:24px;'>
                            Thank you for choosing us. We look forward to hosting you!
                        </p>
                    </div>
                    <div style='background-color:#f9f9f9;padding:16px;text-align:center;border-top:1px solid #e0e0e0;'>
                        <p style='color:#999;font-size:12px;margin:0;'>© 2024 Hotel Booking System. All rights reserved.</p>
                    </div>
                </div>";

            await SendEmailAsync(toEmail, fullName, subject, body);
        }
    }
}