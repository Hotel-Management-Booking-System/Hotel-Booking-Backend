using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace HotelBookingAPI.Models
{
    public class Promotion
    {
        [Key]
        public int PromotionId { get; set; }

        // 🏷️ Promo Code (e.g., SUMMER20)
        [Required]
        [MaxLength(50)]
        [JsonPropertyName("code")]
        public string Code { get; set; }

        // 📝 Description
        [JsonPropertyName("description")]
        public string Description { get; set; }

        // 💰 Discount Percentage (e.g., 10, 20)
        [Range(0, 100)]
        [JsonPropertyName("discountPercent")]
        public decimal DiscountPercentage { get; set; }

        // 💸 OR Flat Discount Amount (optional)
        [JsonPropertyName("flatDiscount")]
        public decimal? FlatDiscountAmount { get; set; }

        // 📅 Validity
        [Required]
        [JsonPropertyName("startDate")]
        public DateTime StartDate { get; set; }

        [Required]
        [JsonPropertyName("endDate")]
        public DateTime EndDate { get; set; }

        // 🔢 Usage Limit (optional)
        [JsonPropertyName("usageLimit")]
        public int? UsageLimit { get; set; }

        public int TimesUsed { get; set; } = 0;

        // 🟢 Active or not
        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; } = true;
        // 🔗 Navigation (Bookings using this promo)
        public ICollection<Booking> Bookings { get; set; }
    }
}