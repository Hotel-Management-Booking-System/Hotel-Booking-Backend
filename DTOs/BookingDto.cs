using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs
{
    public class BookingDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }
    }
}