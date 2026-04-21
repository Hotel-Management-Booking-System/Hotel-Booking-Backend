public class HotelDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string Description { get; set; }
    public string ImageUrl { get; set; }

    public List<int> AmenityIds { get; set; }
}