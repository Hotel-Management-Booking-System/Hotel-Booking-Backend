using HotelBookingWebsite.DTOs;
using HotelBookingWebsite.Models;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetByHotel(int hotelId);
    Task<Room> GetById(int id);
    Task<Room> Create(RoomDto dto);
    Task<Room> Update(int id, RoomDto dto);
    Task Delete(int id);
}