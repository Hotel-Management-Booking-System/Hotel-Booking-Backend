public class HotelDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Description { get; set; }
    public int StarRating { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public string ImageUrl { get; set; }

    public List<int> AmenityIds { get; set; }
}