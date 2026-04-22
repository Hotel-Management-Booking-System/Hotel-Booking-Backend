using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs;
using HotelBookingAPI.Interfaces;
using HotelBookingAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<BookingService> _logger;
    private readonly IEmailService _emailService;

    public BookingService(AppDbContext context, ILogger<BookingService> logger, IEmailService emailService)
    {
        _context = context;
        _logger = logger;
        _emailService = emailService;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(int userId, BookingDto dto)
    {
        _logger.LogInformation("Creating booking for UserId {UserId}, RoomId {RoomId}", userId, dto.RoomId);

        if (dto.CheckInDate >= dto.CheckOutDate)
        {
            _logger.LogWarning("Invalid date range for UserId {UserId}", userId);
            throw new ArgumentException("Check-out date must be after check-in date.");
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
        if (user == null)
        {
            _logger.LogError("Invalid user ID {UserId}", userId);
            throw new ArgumentException("Invalid user ID");
        }

        var room = await _context.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == dto.RoomId);

        if (room == null)
        {
            _logger.LogError("Invalid room ID {RoomId}", dto.RoomId);
            throw new ArgumentException("Invalid room ID.");
        }

        bool isBooked = await _context.Bookings.AnyAsync(b =>
            b.RoomId == dto.RoomId &&
            b.Status != BookingStatus.Cancelled &&
            dto.CheckInDate < b.CheckOutDate &&
            dto.CheckOutDate > b.CheckInDate);

        if (isBooked)
        {
            _logger.LogWarning("Room not available for RoomId {RoomId}", dto.RoomId);
            throw new InvalidOperationException("Room is not available for the selected dates.");
        }

        int totalNights = (dto.CheckOutDate - dto.CheckInDate).Days;
        decimal totalPrice = totalNights * room.Price;

        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            NumberOfGuests = dto.NumberOfGuests,
            TotalPrice = totalPrice,
            Status = BookingStatus.Confirmed,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking created successfully. BookingId {BookingId}", booking.BookingId);

        // Send booking confirmation email
        await _emailService.SendBookingConfirmationEmailAsync(user.Email, user.FullName, new BookingConfirmationEmailDto
        {
            HotelName = room.Hotel.Name,
            RoomType = room.RoomType,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice
        });

        return new BookingResponseDto
        {
            HotelName = room.Hotel.Name,
            RoomType = room.RoomType,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            NumberOfGuests = booking.NumberOfGuests,
            TotalPrice = booking.TotalPrice,
            Status = booking.Status.ToString()
        };
    }

    // ---- GetAllBookingsAsync, GetUserBookingsAsync, UpdateBookingStatusAsync unchanged ----
    public async Task<IEnumerable<BookingResponseDto>> GetAllBookingsAsync()
    {
        _logger.LogInformation("Retrieving all bookings");

        return await _context.Bookings
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Select(b => new BookingResponseDto
            {
                HotelName = b.Room.Hotel.Name,
                RoomType = b.Room.RoomType,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                NumberOfGuests = b.NumberOfGuests,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<BookingResponseDto>> GetUserBookingsAsync(int userId)
    {
        _logger.LogInformation("Retrieving bookings for UserId {UserId}", userId);

        return await _context.Bookings
            .Where(b => b.UserId == userId)
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Select(b => new BookingResponseDto
            {
                HotelName = b.Room.Hotel.Name,
                RoomType = b.Room.RoomType,
                CheckInDate = b.CheckInDate,
                CheckOutDate = b.CheckOutDate,
                NumberOfGuests = b.NumberOfGuests,
                TotalPrice = b.TotalPrice,
                Status = b.Status.ToString()
            })
            .ToListAsync();
    }

    public async Task<bool> UpdateBookingStatusAsync(BookingStatusDto dto)
    {
        _logger.LogInformation("Updating booking status for BookingId {BookingId} to {Status}", dto.BookingId, dto.Status);

        var booking = await _context.Bookings.FindAsync(dto.BookingId);

        if (booking == null)
        {
            _logger.LogError("Booking not found for BookingId {BookingId}", dto.BookingId);
            throw new KeyNotFoundException("Booking not found");
        }

        if (!Enum.IsDefined(typeof(BookingStatus), dto.Status))
        {
            _logger.LogWarning("Invalid status value {Status}", dto.Status);
            throw new ArgumentException("Invalid booking status");
        }

        booking.Status = dto.Status;
        await _context.SaveChangesAsync();

        _logger.LogInformation("Booking status updated successfully for BookingId {BookingId}", dto.BookingId);

        return true;
    }
}