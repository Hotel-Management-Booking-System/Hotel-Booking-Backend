using HotelBookingAPI.Data;
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
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
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
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
        };
    }

    // ✅ Create room
    public async Task<RoomResponseDto> Create(RoomDto dto)
    {
        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == dto.HotelId);

        if (!hotelExists)
            throw new ArgumentException("Invalid HotelId");

        var room = new Room
        {
            HotelId = dto.HotelId,
            RoomType = dto.RoomType,
            Price = dto.Price,
            Capacity = dto.Capacity,
            IsAvailable = dto.IsAvailable,
            ImageUrl = dto.ImageUrl
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new RoomResponseDto
        {
            Id = room.Id,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
        };
    }

    // ✅ Update room
    public async Task<RoomResponseDto> Update(int id, RoomDto dto)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        room.RoomType = dto.RoomType;
        room.Price = dto.Price;
        room.Capacity = dto.Capacity;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();

        return new RoomResponseDto
        {
            Id = room.Id,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
        };
    }
    public async Task<IEnumerable<RoomResponseDto>> GetAll()
    {
        var rooms = await _context.Rooms.ToListAsync();

        return rooms.Select(r => new RoomResponseDto
        {
            Id = r.Id,
            RoomType = r.RoomType,
            Price = r.Price,
            Capacity = r.Capacity,
            IsAvailable = r.IsAvailable
        });
    }

    public async Task Delete(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();


        _logger.LogInformation("Room deleted successfully with Id: {RoomId}", id);
    }

    }




    
