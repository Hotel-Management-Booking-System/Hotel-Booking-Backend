using HotelBookingAPI.Data;
using HotelBookingAPI.Models;
using HotelBookingWebsite.DTOs;
using HotelBookingWebsite.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(AppDbContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    // ✅ Get rooms by hotel
    public async Task<IEnumerable<RoomResponseDto>> GetByHotel(int hotelId)
    {
        var rooms = await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();

        return rooms.Select(room => new RoomResponseDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl
        });
    }

    // ✅ Get single room
    public async Task<RoomResponseDto> GetById(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        return new RoomResponseDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl
        };
    }

    // ✅ Create room
    public async Task<RoomResponseDto> Create(RoomDto dto)
    {
        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == dto.HotelId);

        if (!hotelExists)
            throw new ArgumentException("Invalid HotelId");
        // ✅ Check if room number already exists in the same hotel
        var roomExists = await _context.Rooms
            .AnyAsync(r => r.HotelId == dto.HotelId && r.RoomNumber == dto.RoomNumber);

        if (roomExists)
            throw new InvalidOperationException($"Room number '{dto.RoomNumber}' already exists in this hotel.");



        var room = new Room
        {
            HotelId = dto.HotelId,
            RoomNumber = dto.RoomNumber,
            RoomType = dto.RoomType,
            Price = dto.Price,
            Capacity = dto.Capacity,
            Description = dto.Description,
            IsAvailable = dto.IsAvailable,
            ImageUrl = dto.ImageUrl
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomResponseDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl
        };
    }

    // ✅ Update room
    public async Task<RoomResponseDto> Update(int id, RoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        room.RoomNumber = dto.RoomNumber;
        room.RoomType = dto.RoomType;
        room.Price = dto.Price;
        room.Capacity = dto.Capacity;
        room.Description = dto.Description;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();

        return new RoomResponseDto
        {
            Id = room.Id,
            RoomNumber = room.RoomNumber,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            Description = room.Description,
            IsAvailable = room.IsAvailable,
            ImageUrl = room.ImageUrl
        };
    }
    public async Task<IEnumerable<RoomResponseDto>> GetAll()
    {
        var rooms = await _context.Rooms.ToListAsync();

        return rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            RoomType = r.RoomType,
            Price = r.Price,
            Capacity = r.Capacity,
            Description = r.Description,
            IsAvailable = r.IsAvailable,
            ImageUrl = r.ImageUrl
        });
    }

    public async Task Delete(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        // Check for active bookings
        bool hasActiveBookings = await _context.Bookings.AnyAsync(b => b.RoomId == id && b.Status != BookingStatus.Cancelled);
        if (hasActiveBookings)
        {
            _logger.LogWarning("Delete failed. Room {RoomId} has active bookings.", id);
            throw new InvalidOperationException("Cannot delete room with active bookings.");
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();


        _logger.LogInformation("Room deleted successfully with Id: {RoomId}", id);
    }

    }




    
