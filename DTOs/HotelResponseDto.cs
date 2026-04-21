namespace HotelBookingWebsite.DTOs
{
    public class HotelResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public List<string> Amenities { get; set; }
    }
}
