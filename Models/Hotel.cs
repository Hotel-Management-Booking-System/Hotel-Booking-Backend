namespace HotelBookingWebsite.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public string Location { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;

        public string Description { get; set; }
        public int StarRating { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; } = string.Empty; 
        public ICollection<Room> Rooms { get; set; }
        public ICollection<Amenity> Amenities { get; set; }
    }
}