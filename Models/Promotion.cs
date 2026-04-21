using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HotelBookingAPI.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        // 🏷️ Promo Code (e.g., SUMMER20)
        [Required]
        [MaxLength(50)]
        public string Code { get; set; }
        // 📝 Description
        public string Description { get; set; }
        // 💰 Discount Percentage (e.g., 10, 20)
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; }
        // 💸 OR Flat Discount Amount (optional)
        public decimal? FlatDiscountAmount { get; set; }
        // 📅 Validity
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        // 🔢 Usage Limit (optional)
        public int? UsageLimit { get; set; }
        public int TimesUsed { get; set; } = 0;
        // 🟢 Active or not
        public bool IsActive { get; set; } = true;
        // 🔗 Navigation (Bookings using this promo)
        public ICollection<Booking> Bookings { get; set; }
    }
}