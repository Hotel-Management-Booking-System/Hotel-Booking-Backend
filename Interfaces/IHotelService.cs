using HotelBookingWebsite.DTOs;
using HotelBookingWebsite.Models;

public interface IHotelService
{
    Task<IEnumerable<HotelResponseDto>> GetAll();
    Task<Hotel> GetById(int id);
    Task<Hotel> Create(HotelDto dto);
    Task<Hotel> Update(int id, HotelDto dto);
    Task Delete(int id);

    Task<IEnumerable<Hotel>> Search(string? city, decimal? minPrice, decimal? maxPrice, List<int>? amenityIds);

    Task<Amenity> AddAmenityAsync(string name);
}