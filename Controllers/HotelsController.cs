using HotelBookingWebsite.DTOs;
using HotelBookingWebsite.Models;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize] //All endpoints require authentication
public class HotelsController : ControllerBase
{
    private readonly IHotelService _service;

    public HotelsController(IHotelService service)
    {
        _service = service;
    }

    //  Admin + Customer
    [HttpGet]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    //  Admin + Customer
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

    //  ONLY Admin
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create(HotelDto dto)
    {
        return Ok(await _service.Create(dto));
    }

    //  ONLY Admin
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, HotelDto dto)
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

    // ONLY Admin
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

    //  Admin + Customer
    [HttpGet("search")]
    [Authorize(Roles = "Admin,Customer")]
    public async Task<IActionResult> Search(
        string? city,
        decimal? minPrice,
        decimal? maxPrice,
        [FromQuery] List<int>? amenityIds)
    {
        return Ok(await _service.Search(city, minPrice, maxPrice, amenityIds));
    }


    [HttpPost("amenity")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAmenity([FromBody] AmenityDto dto)
    {
        var amenity = await _service.AddAmenityAsync(dto.Name);
        return Ok(amenity);
    }





}