using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;
using HotelListingApi.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelListingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountriesController(ICountriesServices countriesServices) : BaseApiController
    {
    
        // GET: api/Countries
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetCountriesDto>>> GetCountries()
        {
            var countries = await countriesServices.GetCountriesAsync();                                                        
            // process countries
            return ToActionResult(countries);
        }

        // GET: api/Countries/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GetCountryDto>> GetCountry(int id)
        {
            var country = await countriesServices.GetCountryAsync(id);
            return ToActionResult(country);
        }

        // PUT: api/Countries/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCountry(int id, UpdateCountryDto updateDto)
        { 
            var result = await countriesServices.UpdateCountryAsync(id, updateDto);
            return ToActionResult(result);
        }

        // POST: api/Countries
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Country>> PostCountry(CreateCountryDto createDto)
        {
            var resultDto = await countriesServices.CreateCountryAsync(createDto);
            if (!resultDto.IsSuccess) return MapErrorsToResponse(resultDto.Errors);
            return CreatedAtAction("GetCountry", new { id = resultDto.Value!.Id}, resultDto);
        }

        // DELETE: api/Countries/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCountry(int id)
        {
           var result = await countriesServices.DeleteCountryAsync(id);
            return ToActionResult(result);
        }
    }
}

