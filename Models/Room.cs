using HotelBookingAPI.Models;

namespace HotelBookingWebsite.Models
{
   public class Room
    {
        public int Id { get; set; }
        public int HotelId { get; set; }

        public string RoomType { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public bool IsAvailable { get; set; }

        public string ImageUrl { get; set; } = string.Empty; 

        public Hotel Hotel { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }
}