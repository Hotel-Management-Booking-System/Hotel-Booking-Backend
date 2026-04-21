namespace HotelBookingWebsite.DTOs
{
    public class RoomResponseDto
    {
        public int Id { get; set; }
        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }
    }
}
