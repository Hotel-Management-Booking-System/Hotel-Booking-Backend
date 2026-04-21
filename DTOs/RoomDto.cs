public class RoomDto
{
    public int HotelId { get; set; }
    public string RoomType { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public bool IsAvailable { get; set; }
    public string ImageUrl { get; set; }
}