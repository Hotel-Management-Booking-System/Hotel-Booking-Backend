namespace HotelBookingAPI.DTOs
{
    public class BookingConfirmationEmailDto
    {
        public string HotelName { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
    }
}