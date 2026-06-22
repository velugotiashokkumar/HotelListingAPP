using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Hotel;
using HotelListingApi.Contracts;


namespace HotelListingApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelsServices hotelsServices) : BaseApiController
{

    // GET: api/Hotel
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetHotelDto>>> GetHotel()
    {
        var hotels = await hotelsServices.GetHotelsAsync();

        return ToActionResult(hotels);
    }

    // GET: api/Hotel/5
    [HttpGet("{id}")]
    public async Task<ActionResult<GetHotelDto>> GetHotel(int id)
    {
        var hotel = await hotelsServices.GetHotelAsync(id);
        
        return ToActionResult(hotel);
    }

    // PUT: api/Hotel/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutHotel(int id, UpdateHotelDto hotelDto)
    {
        if (id != hotelDto.Id)
        {
            return BadRequest("Id route value must payload Id.");
        }

        var result = await hotelsServices.UpdateHotelAsync(id, hotelDto);
        return ToActionResult(result);
    }

    // POST: api/Hotel
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<GetHotelDto>> PostHotel(CreateHotelDto hotelDto)
    {
        
        var hotel = await hotelsServices.CreateHotelAsync(hotelDto);
        if (!hotel.IsSuccess) return MapErrorsToResponse(hotel.Errors);

        return CreatedAtAction(nameof(GetHotel), new { id = hotel.Value!.Id }, hotel.Value);

    }

    // DELETE: api/Hotel/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        var result = await hotelsServices.DeleteHotelAsync(id);
        return ToActionResult(result);
    }
}
