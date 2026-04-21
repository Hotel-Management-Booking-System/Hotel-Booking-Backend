using Microsoft.EntityFrameworkCore;
using HotelBookingAPI.Models;
using HotelBookingWebsite.Models;

namespace HotelBookingAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // 👤 Users
        public DbSet<User> Users { get; set; }

        // 🏨 Hotels
        public DbSet<Hotel> Hotels { get; set; }

        // 🛏️ Rooms
        public DbSet<Room> Rooms { get; set; }

        // 📅 Bookings
        public DbSet<Booking> Bookings { get; set; }

        // 🎁 Promotions
        public DbSet<Promotion> Promotions { get; set; }

        // 🧰 Amenities
        public DbSet<Amenity> Amenities { get; set; }



        }
    }
