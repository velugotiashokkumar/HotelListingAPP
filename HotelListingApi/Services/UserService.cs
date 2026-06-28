using HotelListingApi.Constants;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Auth;
using HotelListingApi.Results;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListingApi.Services
{
    public class UserService(UserManager<ApplicationUser> userManager, IConfiguration configuration) : IUserService
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

            await userManager.AddToRoleAsync(user, registerUserDto.Role);

            var registeredUser = new RegisterdUserDto
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Id = user.Id,
                Role = registerUserDto.Role
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

            // Issue a token
            var token = await GenerateToken(user);

            return Result<string>.Success(token);
        }

        private async Task<string> GenerateToken(ApplicationUser user)
        {
            // Set basic user claims
            var claims = new List<Claim>
            {
                new (JwtRegisteredClaimNames.Sub, user.Id),
                new (JwtRegisteredClaimNames.Email, user.Email),
                new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new (JwtRegisteredClaimNames.Name, user.FullName),
            };

            //Set user role claims
            var roles = await userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            claims = claims.Union(roleClaims).ToList();

            // Set JWT Key credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create an encoded token
            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(configuration["JwtSettings:DurationInMinutes"])),
                signingCredentials: credentials
                );

            // Return token value
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
