using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;
using HotelListingApi.Results;

namespace HotelListingApi.Contracts
{
    public interface IHotelsServices
    {
        Task<bool> HotelExistsAsync(int id);
        Task<bool> HotelExistsAsync(string name, int countryId);
        Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto createDto);
        Task<Result> DeleteHotelAsync(int id);
        Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync();
        Task<Result<GetHotelDto>> GetHotelAsync(int id);
        Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto);
    }
}
