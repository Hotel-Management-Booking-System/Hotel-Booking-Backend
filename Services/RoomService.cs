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

    public async Task<IEnumerable<RoomResponseDto>> GetByHotel(int hotelId)
    {
        _logger.LogInformation("Fetching rooms for HotelId: {HotelId}", hotelId);

        var rooms = await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();

        _logger.LogInformation("Fetched {Count} rooms for HotelId: {HotelId}", rooms.Count, hotelId);

        return rooms.Select(room => new RoomResponseDto
        {
            Id = room.Id,
            RoomType = room.RoomType,
            Price = room.Price,
            Capacity = room.Capacity,
            IsAvailable = room.IsAvailable
        });
    }

    public async Task<Room> GetById(int id)
    {
        _logger.LogInformation("Fetching room with Id: {RoomId}", id);

        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            _logger.LogWarning("Room not found with Id: {RoomId}", id);
            throw new KeyNotFoundException("Room not found");
        }

        return room;
    }

    public async Task<Room> Create(RoomDto dto)
    {
        _logger.LogInformation("Creating room for HotelId: {HotelId}", dto.HotelId);

        var hotelExists = await _context.Hotels.AnyAsync(h => h.Id == dto.HotelId);

        if (!hotelExists)
        {
            _logger.LogWarning("Room creation failed. Invalid HotelId: {HotelId}", dto.HotelId);
            throw new ArgumentException("Invalid HotelId");
        }

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

        _logger.LogInformation("Room created successfully with Id: {RoomId}", room.Id);

        return room;
    }

    public async Task<Room> Update(int id, RoomDto dto)
    {
        _logger.LogInformation("Updating room with Id: {RoomId}", id);

        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            _logger.LogWarning("Update failed. Room not found with Id: {RoomId}", id);
            throw new KeyNotFoundException("Room not found");
        }

        room.RoomType = dto.RoomType;
        room.Price = dto.Price;
        room.Capacity = dto.Capacity;
        room.IsAvailable = dto.IsAvailable;
        room.ImageUrl = dto.ImageUrl;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Room updated successfully with Id: {RoomId}", id);

        return room;
    }

    public async Task Delete(int id)
    {
        _logger.LogInformation("Deleting room with Id: {RoomId}", id);

        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
        {
            _logger.LogWarning("Delete failed. Room not found with Id: {RoomId}", id);
            throw new KeyNotFoundException("Room not found");
        }

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Room deleted successfully with Id: {RoomId}", id);
    }


}