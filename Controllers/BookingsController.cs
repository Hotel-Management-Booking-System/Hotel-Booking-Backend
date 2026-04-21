using HotelBookingAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingsController> _logger;
        public BookingsController(IBookingService bookingService, ILogger<BookingsController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            _logger.LogInformation("CreateBooking API called");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var result = await _bookingService.CreateBookingAsync(userId, dto);
            return Ok(result);
        }

        [HttpGet("my-bookings")]

        public async Task<IActionResult> GetUserBookings()
        {
            _logger.LogInformation("Fetching bookings for user");
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var bookings = await _bookingService.GetUserBookingsAsync(userId);
            return Ok(bookings);

        }
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllBookings()
        {
            _logger.LogInformation("Admin fetching all bookings");

            var bookings = await _bookingService.GetAllBookingsAsync();

            return Ok(bookings);
        }

        [HttpPut("status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBookingStatus([FromBody] BookingStatusDto dto)
        {
            _logger.LogInformation("Updating booking status");

            var result = await _bookingService.UpdateBookingStatusAsync(dto);

            return Ok(new { message = "Booking status updated successfully" });
        }


    }
}