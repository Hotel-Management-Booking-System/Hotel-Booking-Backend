using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] // all endpoints require login
public class RoomsController : ControllerBase
{
    private readonly IRoomService _service;

    public RoomsController(IRoomService service)
    {
        _service = service;
    }

    //  Admin + Customer
    [HttpGet("hotel/{hotelId}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetByHotel(int hotelId)
    {
        return Ok(await _service.GetByHotel(hotelId));
    }

    // ✅ Admin: Get all rooms
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllRooms()
    {
        return Ok(await _service.GetAll());
    }

    // Admin + Customer
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> Get(int id)
    {
        try
        {
            return Ok(await _service.GetById(id));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    // ONLY Admin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(RoomDto dto)
    {
        try
        {
            return Ok(await _service.Create(dto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);          // Room already exists
        }
    }

    //  ONLY Admin
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, RoomDto dto)
    {
        try
        {
            return Ok(await _service.Update(id, dto));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    //  ONLY Admin
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _service.Delete(id);
            return Ok("Deleted successfully");
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}