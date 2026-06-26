using HotelListingApi.Constants;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Auth;
using HotelListingApi.Results;
using Microsoft.AspNetCore.Identity;

namespace HotelListingApi.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        public async Task<Result<RegisterdUserDto>> RegisterAsync(RegisterUserDto registerUserDto)
        {
            var user = new ApplicationUser
            {
                Email = registerUserDto.Email,
                FirstName = registerUserDto.FirstName,
                LastName = registerUserDto.LastName,
                UserName = registerUserDto.Email
            };

            var result = await userManager.CreateAsync(user, registerUserDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => new Error(ErrorCodes.BadRequest, e.Description)).ToArray();
                return Result<RegisterdUserDto>.BadRequest(errors);
            }

            var registeredUser = new RegisterdUserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id
            };
            return Result<RegisterdUserDto>.Success(registeredUser);
        }

        public async Task<Result<string>> LoginAsync(LoginUserDto loginUserDto)
        {
            var user = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (user == null)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials"));
            }

            var valid = await userManager.CheckPasswordAsync(user, loginUserDto.Password);
            if (!valid)
            {
                return Result<string>.Failure(new Error(ErrorCodes.BadRequest, "Invalid credentials"));
            }

            return Result<string>.Success("Login Successfull.");
        }
    }
}
