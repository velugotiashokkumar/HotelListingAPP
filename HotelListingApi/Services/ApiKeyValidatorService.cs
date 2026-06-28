using HotelListingApi.Contracts;
using HotelListingApi.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
namespace HotelListingApi.Services
{
    public class ApiKeyValidatorService(HotelListingDbContext db) : IApiKeyValidatorService
    {
        public async Task<bool> IsValidAsync(string apiKey, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(apiKey)) return false;

            var apiKeyEntity = await db.ApiKeys
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.Key == apiKey, ct);
            
            if (apiKeyEntity == null) return false;
            // If there is noexpiry date or the expiry date does not exceed today's date.
            return apiKeyEntity.IsActive;
        }
    }
}
