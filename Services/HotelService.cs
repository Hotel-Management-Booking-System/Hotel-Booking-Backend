using HotelBookingAPI.Data;
using HotelBookingWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class HotelService : IHotelService
{
    private readonly AppDbContext _context;
    private readonly ILogger<HotelService> _logger;

    public HotelService(AppDbContext context, ILogger<HotelService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<Hotel>> GetAll()
    {
        _logger.LogInformation("Fetching all hotels");

        var hotels = await _context.Hotels
            .Include(h => h.Amenities)
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} hotels", hotels.Count);

        return hotels;
    }

    public async Task<Hotel> GetById(int id)
    {
        _logger.LogInformation("Fetching hotel with Id: {HotelId}", id);

        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
        {
            _logger.LogWarning("Hotel not found with Id: {HotelId}", id);
            throw new KeyNotFoundException("Hotel not found");
        }

        return hotel;
    }

    public async Task<Hotel> Create(HotelDto dto)
    {
        _logger.LogInformation("Creating hotel: {HotelName}", dto.Name);

        var amenities = await _context.Amenities
            .Where(a => dto.AmenityIds.Contains(a.Id))
            .ToListAsync();

        var hotel = new Hotel
        {
            Name = dto.Name,
            Location = dto.Location,
            City = dto.City,
            Country = dto.Country,
            Description = dto.Description,
            ImageUrl = dto.ImageUrl,
            Amenities = amenities
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel created successfully with Id: {HotelId}", hotel.Id);

        return hotel;
    }

    public async Task<Hotel> Update(int id, HotelDto dto)
    {
        _logger.LogInformation("Updating hotel with Id: {HotelId}", id);

        var hotel = await _context.Hotels
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
        {
            _logger.LogWarning("Update failed. Hotel not found with Id: {HotelId}", id);
            throw new KeyNotFoundException("Hotel not found");
        }

        hotel.Name = dto.Name;
        hotel.Location = dto.Location;
        hotel.City = dto.City;
        hotel.Country = dto.Country;
        hotel.Description = dto.Description;
        hotel.ImageUrl = dto.ImageUrl;

        hotel.Amenities = await _context.Amenities
            .Where(a => dto.AmenityIds.Contains(a.Id))
            .ToListAsync();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel updated successfully with Id: {HotelId}", id);

        return hotel;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation("Deleting hotel with Id: {HotelId}", id);

        var hotel = await _context.Hotels.FindAsync(id);

        if (hotel == null)
        {
            _logger.LogWarning("Delete failed. Hotel not found with Id: {HotelId}", id);
            throw new KeyNotFoundException("Hotel not found");
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel deleted successfully with Id: {HotelId}", id);
    }

    public async Task<IEnumerable<Hotel>> Search(string? city, decimal? minPrice, decimal? maxPrice, List<int>? amenityIds)
    {
        _logger.LogInformation(
            "Searching hotels with filters - City: {City}, MinPrice: {MinPrice}, MaxPrice: {MaxPrice}, Amenities: {AmenitiesCount}",
            city, minPrice, maxPrice, amenityIds?.Count ?? 0
        );

        var query = _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Amenities)
            .AsQueryable();

        if (!string.IsNullOrEmpty(city))
            query = query.Where(h => h.City == city);

        if (minPrice.HasValue)
            query = query.Where(h => h.Rooms.Any(r => r.Price >= minPrice));

        if (maxPrice.HasValue)
            query = query.Where(h => h.Rooms.Any(r => r.Price <= maxPrice));

        if (amenityIds != null && amenityIds.Any())
            query = query.Where(h => h.Amenities.Any(a => amenityIds.Contains(a.Id)));

        var result = await query.ToListAsync();

        _logger.LogInformation("Search returned {Count} hotels", result.Count);

        return result;
    }
}