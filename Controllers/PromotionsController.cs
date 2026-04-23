using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromotionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PromotionsController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ Get all promotions (Admin)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            var promotions = await _context.Promotions
                .Select(p => new {
                    id = p.PromotionId,
                    code = p.Code,
                    description = p.Description,
                    discountPercent = p.DiscountPercentage,
                    startDate = p.StartDate,
                    endDate = p.EndDate,
                    isActive = p.IsActive
                })
                .ToListAsync();
            return Ok(promotions);
        }

        // ✅ Create promotion (Admin)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] Promotion promotion)
        {
            if (await _context.Promotions.AnyAsync(p => p.Code == promotion.Code))
                return BadRequest("Promotion code already exists.");

            await _context.Promotions.AddAsync(promotion);
            await _context.SaveChangesAsync();
            return Ok(promotion);
        }

        // ✅ Update promotion (Admin)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, [FromBody] Promotion promo)
        {
            var existing = await _context.Promotions.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Code = promo.Code;
            existing.Description = promo.Description;
            existing.DiscountPercentage = promo.DiscountPercentage;
            existing.FlatDiscountAmount = promo.FlatDiscountAmount;
            existing.StartDate = promo.StartDate;
            existing.EndDate = promo.EndDate;
            existing.IsActive = promo.IsActive;
            existing.UsageLimit = promo.UsageLimit;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        // ✅ Delete promotion (Admin)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var promo = await _context.Promotions.FindAsync(id);
            if (promo == null) return NotFound();

            _context.Promotions.Remove(promo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Promotion deleted successfully" });
        }

        // ✅ Validate promotion code (Used during booking)
        [HttpGet("validate/{code}")]
        public async Task<IActionResult> Validate(string code)
        {
            var promotion = await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code && p.IsActive && 
                                         DateTime.UtcNow >= p.StartDate && DateTime.UtcNow <= p.EndDate);

            if (promotion == null)
                return NotFound("Invalid or expired promotion code.");

            if (promotion.UsageLimit.HasValue && promotion.TimesUsed >= promotion.UsageLimit.Value)
                return BadRequest("Promotion code usage limit reached.");

            // Return a DTO or the promotion object (matching frontend expectation)
            return Ok(new {
                id = promotion.PromotionId,
                code = promotion.Code,
                discountPercent = promotion.DiscountPercentage,
                flatDiscount = promotion.FlatDiscountAmount,
                description = promotion.Description
            });
        }
    }
}
