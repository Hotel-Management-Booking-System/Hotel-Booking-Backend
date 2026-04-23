using HotelBookingAPI.DTOs;

namespace HotelBookingAPI.Interfaces
{
    public interface IBookingService
    {
        Task<BookingResponseDto> CreateBookingAsync(int userId, BookingDto dto);

        Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId);

        Task<bool> UpdateBookingStatusAsync(BookingStatusDto dto);

        Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync();
    }
}