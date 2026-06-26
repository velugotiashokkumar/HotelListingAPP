using HotelListingApi.DTOs.Auth;
using HotelListingApi.Results;

namespace HotelListingApi.Contracts
{
    public interface IUserService
    {
        Task<Result<string>> LoginAsync(LoginUserDto loginUserDto);
        Task<Result<RegisterdUserDto>> RegisterAsync(RegisterUserDto registerUserDto);
    }
}