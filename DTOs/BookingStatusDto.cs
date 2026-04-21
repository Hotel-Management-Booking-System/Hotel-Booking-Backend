using HotelBookingAPI.Models;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs
{
    public class BookingStatusDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        public BookingStatus Status { get; set; }
        // Suggested: Confirmed / Cancelled
    }
}