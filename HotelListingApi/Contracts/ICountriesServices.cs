using HotelListingApi.DTOs.Country;
using HotelListingApi.Results;

namespace HotelListingApi.Contracts
{
    public interface ICountriesServices
    {
        Task<bool> CountryExistsAsync(int id);
        Task<bool> CountryExistsAsync(string name);
        Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto createDto);
        Task<Result> DeleteCountryAsync(int id);
        Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync();
        Task<Result<GetCountryDto>> GetCountryAsync(int id);
        Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto);
    }
}