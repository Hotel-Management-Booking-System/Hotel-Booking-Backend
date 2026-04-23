namespace HotelBookingWebsite.DTOs
{
    public class HotelResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public int StarRating { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; }
        public List<string> Amenities { get; set; }
    }
}
