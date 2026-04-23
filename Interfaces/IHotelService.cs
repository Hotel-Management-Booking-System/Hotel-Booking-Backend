using HotelBookingWebsite.DTOs;
using HotelBookingWebsite.Models;

public interface IHotelService
{
    Task<IEnumerable<HotelResponseDto>> GetAll();
    Task<HotelResponseDto> GetById(int id);
    Task<HotelResponseDto> Create(HotelDto dto);
    Task<HotelResponseDto> Update(int id, HotelDto dto);
    Task Delete(int id);

    Task<IEnumerable<HotelResponseDto>> Search(string? city, decimal? minPrice, decimal? maxPrice, List<int>? amenityIds);

    Task<Amenity> AddAmenityAsync(string name);
    Task<IEnumerable<Amenity>> GetAllAmenitiesAsync();
}