using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingWebsite.DTOs;
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

    public async Task<IEnumerable<HotelResponseDto>> GetAll()
    {
        _logger.LogInformation("Fetching all hotels");

        var hotels = await _context.Hotels
            .Include(h => h.Amenities)
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} hotels", hotels.Count);

        return hotels.Select(h => new HotelResponseDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Location = h.Location,
            City = h.City,
            Country = h.Country,
            StarRating = h.StarRating,
            PhoneNumber = h.PhoneNumber,
            Email = h.Email,
            ImageUrl = h.ImageUrl,
            Amenities = h.Amenities.Select(a => a.Name).ToList()
        });
    }

    public async Task<HotelResponseDto> GetById(int id)
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

        return new HotelResponseDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Location = hotel.Location,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PhoneNumber = hotel.PhoneNumber,
            Email = hotel.Email,
            ImageUrl = hotel.ImageUrl,
            Amenities = hotel.Amenities.Select(a => a.Name).ToList()
        };
    }

    public async Task<HotelResponseDto> Create(HotelDto dto)
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
            StarRating = dto.StarRating,
            PhoneNumber = dto.PhoneNumber,
            Email = dto.Email,
            ImageUrl = dto.ImageUrl,
            Amenities = amenities
        };

        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel created successfully with Id: {HotelId}", hotel.Id);

        return new HotelResponseDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Location = hotel.Location,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PhoneNumber = hotel.PhoneNumber,
            Email = hotel.Email,
            ImageUrl = hotel.ImageUrl,
            Amenities = hotel.Amenities.Select(a => a.Name).ToList()
        };
    }

    public async Task<HotelResponseDto> Update(int id, HotelDto dto)
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
        hotel.StarRating = dto.StarRating;
        hotel.PhoneNumber = dto.PhoneNumber;
        hotel.Email = dto.Email;
        hotel.ImageUrl = dto.ImageUrl;

        hotel.Amenities = await _context.Amenities
            .Where(a => dto.AmenityIds.Contains(a.Id))
            .ToListAsync();

        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel updated successfully with Id: {HotelId}", id);

        return new HotelResponseDto
        {
            Id = hotel.Id,
            Name = hotel.Name,
            Description = hotel.Description,
            Location = hotel.Location,
            City = hotel.City,
            Country = hotel.Country,
            StarRating = hotel.StarRating,
            PhoneNumber = hotel.PhoneNumber,
            Email = hotel.Email,
            ImageUrl = hotel.ImageUrl,
            Amenities = hotel.Amenities.Select(a => a.Name).ToList()
        };
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation("Deleting hotel with Id: {HotelId}", id);

        var hotel = await _context.Hotels.Include(h => h.Rooms).FirstOrDefaultAsync(h => h.Id == id);

        if (hotel == null)
        {
            _logger.LogWarning("Delete failed. Hotel not found with Id: {HotelId}", id);
            throw new KeyNotFoundException("Hotel not found");
        }

        // Check for active bookings in any of the hotel's rooms
        bool hasActiveBookings = await _context.Bookings.AnyAsync(b => b.Room.HotelId == id && b.Status != BookingStatus.Cancelled);
        if (hasActiveBookings)
        {
            _logger.LogWarning("Delete failed. Hotel {HotelId} has active bookings.", id);
            throw new InvalidOperationException("Cannot delete hotel with active bookings.");
        }

        _context.Hotels.Remove(hotel);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Hotel deleted successfully with Id: {HotelId}", id);
    }

    public async Task<Amenity> AddAmenityAsync(string name)
    {
        _logger.LogInformation("Adding new amenity: {AmenityName}", name);

        if (string.IsNullOrWhiteSpace(name))
        {
            _logger.LogWarning("Amenity name is empty");
            throw new ArgumentException("Amenity name cannot be empty");
        }
        // 🔹 Check duplicate
        var exists = await _context.Amenities
            .AnyAsync(a => a.Name.ToLower() == name.ToLower());

        if (exists)
        {
            _logger.LogWarning("Amenity already exists: {AmenityName}", name);
            throw new InvalidOperationException("Amenity already exists");
        }

        var amenity = new Amenity
        {
            Name = name
        };

        await _context.Amenities.AddAsync(amenity);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Amenity added successfully with Id {AmenityId}", amenity.Id);

        return amenity;
    }
    public async Task<IEnumerable<Amenity>> GetAllAmenitiesAsync()
    {
        _logger.LogInformation("Fetching all amenities");
        return await _context.Amenities.ToListAsync();
    }


    public async Task<IEnumerable<HotelResponseDto>> Search(string? city, decimal? minPrice, decimal? maxPrice, List<int>? amenityIds)
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

        var filteredHotels = await query.ToListAsync();

        _logger.LogInformation("Search returned {Count} hotels", filteredHotels.Count);

        return filteredHotels.Select(h => new HotelResponseDto
        {
            Id = h.Id,
            Name = h.Name,
            Description = h.Description,
            Location = h.Location,
            City = h.City,
            Country = h.Country,
            StarRating = h.StarRating,
            PhoneNumber = h.PhoneNumber,
            Email = h.Email,
            ImageUrl = h.ImageUrl,
            Amenities = h.Amenities.Select(a => a.Name).ToList()
        });
    }

}