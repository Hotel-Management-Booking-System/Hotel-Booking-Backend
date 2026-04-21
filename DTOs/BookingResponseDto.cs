using System;

namespace HotelBookingAPI.DTOs
{
    public class BookingResponseDto
    {
        public string BookingNumber { get; set; }

        public string HotelName { get; set; }

        public string RoomType { get; set; }

        public DateTime CheckInDate { get; set; }

        public DateTime CheckOutDate { get; set; }

        public int NumberOfGuests { get; set; }

        public decimal TotalPrice { get; set; }

        public string Status { get; set; }
    }
}