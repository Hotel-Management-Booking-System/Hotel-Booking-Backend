using HotelBookingAPI.Data;
using HotelBookingWebsite.Models;
using Microsoft.EntityFrameworkCore;

public class HotelService : IHotelService
{
    private readonly AppDbContext _context;

    public HotelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hotel>> GetAll()
    {
        return await _context.Hotels
            .Include(h => h.Amenities)
            .ToListAsync();
    }

    public async Task<Hotel> GetById(int id)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Rooms)
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            throw new KeyNotFoundException("Hotel not found");

        return hotel;
    }

    public async Task<Hotel> Create(HotelDto dto)
    {
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

        return hotel;
    }

    public async Task<Hotel> Update(int id, HotelDto dto)
    {
        var hotel = await _context.Hotels
            .Include(h => h.Amenities)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
            throw new KeyNotFoundException("Hotel not found");

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
        return hotel;
    }

    public async Task Delete(int id)
    {
        var hotel = await _context.Hotels.FindAsync(id);
        if (hotel == null)
            throw new KeyNotFoundException("Hotel not found");

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Hotel>> Search(string? city, decimal? minPrice, decimal? maxPrice, List<int>? amenityIds)
    {
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

        return await query.ToListAsync();
    }
}