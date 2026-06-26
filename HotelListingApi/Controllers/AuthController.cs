using HotelListingApi.Data;
using HotelListingApi.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.AspNetCore.Identity;
using HotelListingApi.Results;
using HotelListingApi.Constants;
using HotelListingApi.Contracts;

namespace HotelListingApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AuthController(IUserService userService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<RegisterdUserDto>> Register(RegisterUserDto registerUserDto)
        {
            var result = await userService.RegisterAsync(registerUserDto);
            return ToActionResult(result);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login(LoginUserDto LoginUserDto)
        {
            var result = await userService.LoginAsync(LoginUserDto);
            return ToActionResult(result);
        }
    }
}
