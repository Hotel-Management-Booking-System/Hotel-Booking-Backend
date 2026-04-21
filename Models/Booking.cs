using HotelBookingWebsite.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBookingAPI.Models
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled
    }
    public class Booking
    {
        [Key]
        public int BookingId { get; set; }
        // 👤 User who booked
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }

        // 🏨 Room booked
        [Required]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        // 📅 Booking Dates
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        // 👥 Guests
        [Required]
        public int NumberOfGuests { get; set; }
        // 💰 Pricing
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }

        // 📊 Booking Status
        [Required]
        public BookingStatus Status { get; set; } = BookingStatus.Pending;
        // Suggested: Pending, Confirmed, Cancelled
        // 📅 Created Date
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}