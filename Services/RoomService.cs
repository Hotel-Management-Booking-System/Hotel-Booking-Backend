using HotelBookingAPI.Data;
using HotelBookingWebsite.Models;
using Microsoft.EntityFrameworkCore;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Room>> GetByHotel(int hotelId)
    {
        return await _context.Rooms
            .Where(r => r.HotelId == hotelId)
            .ToListAsync();
    }

    public async Task<Room> GetById(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        return room;
    }

    public async Task<Room> Create(RoomDto dto)
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

        return room;
    }

    public async Task<Room> Update(int id, RoomDto dto)
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
        return room;
    }

    public async Task Delete(int id)
    {
        var room = await _context.Rooms.FindAsync(id);

        if (room == null)
            throw new KeyNotFoundException("Room not found");

        _context.Rooms.Remove(room);
        await _context.SaveChangesAsync();
    }
}