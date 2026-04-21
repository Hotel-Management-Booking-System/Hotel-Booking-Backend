using HotelBookingWebsite.DTOs;

using HotelBookingWebsite.Models;

public interface IRoomService
{
    Task<IEnumerable<RoomResponseDto>> GetByHotel(int hotelId);
    Task<RoomResponseDto> GetById(int id);
    Task<RoomResponseDto> Create(RoomDto dto);
    Task<RoomResponseDto> Update(int id, RoomDto dto);
    Task Delete(int id);
    Task<IEnumerable<RoomResponseDto>> GetAll();
}