using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;
using HotelListingApi.Results;
using HotelListingApi.Constants;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace HotelListingApi.Services
{
    public class CountriesServices(HotelListingDbContext context, IMapper mapper) : ICountriesServices
    {
        public async Task<Result<IEnumerable<GetCountriesDto>>> GetCountriesAsync()
        {
            // Without automapper
            //var countries = await context.Countries
            //    .Select(c => new GetCountriesDto(c.CountryId, c.Name, c.ShortName))
            //    .ToListAsync();

            // With Automapper
            var countries = await context.Countries
                .ProjectTo<GetCountriesDto>(mapper.ConfigurationProvider)
                .ToListAsync();

            return Result<IEnumerable<GetCountriesDto>>.Success(countries);
        }

        public async Task<Result<GetCountryDto>> GetCountryAsync(int id)
        {
            // Without automapper
            //var country = await context.Countries
            //    .Where(c => c.CountryId == id)
            //    .Select(c => new GetCountryDto(
            //        c.CountryId,
            //        c.Name,
            //        c.ShortName,
            //        c.Hotels.Select(h => new GetHotelSlimDto(
            //            h.Id,
            //            h.Name,
            //            h.Address,
            //            h.Rating
            //        )).ToList()
            //    ))
            //    .FirstOrDefaultAsync();

            // With Automapper
            var country = await context.Countries
                .Where(q => q.CountryId == id)
                .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return country is null
                ? Result<GetCountryDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{id}' was not found."))
                : Result<GetCountryDto>.Success(country);
        }

        public async Task<Result<GetCountryDto>> CreateCountryAsync(CreateCountryDto createDto)
        {
            try
            {
                var exists = await CountryExistsAsync(createDto.Name);
                if (exists)
                {
                    return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Conflict, $"Country whith name '{createDto.Name}' already exists."));
                }

                // Without Automapper
                //var country = new Country
                //{
                //    Name = createDto.Name,
                //    ShortName = createDto.ShortName
                //};

                // With Automapper
                var country = mapper.Map<Country>(createDto);

                context.Countries.Add(country);
                await context.SaveChangesAsync();

                // Without Automapper
                //var dto = new GetCountryDto(
                //    country.CountryId,
                //    country.Name,
                //    country.ShortName,
                //    []
                //);

                // With Automapper
                var dto = await context.Countries
                    .Where(c => c.CountryId == country.CountryId)
                    .ProjectTo<GetCountryDto>(mapper.ConfigurationProvider)
                    .FirstAsync();

                Console.WriteLine("Hello");

                return Result<GetCountryDto>.Success(dto);
            }
            catch
            {
                return Result<GetCountryDto>.Failure(new Error(ErrorCodes.Failure, "An unexpected error occured while creating the country."));
            }
            
        }

        public async Task<Result> UpdateCountryAsync(int id, UpdateCountryDto updateDto)
        {
            try
            {
                if (id != updateDto.Id)
                {
                    return Result.Failure(new Error("Validation", "Id route value doesnot match payload Id."));
                }

                var country = await context.Countries.FindAsync(id);
                if (country is null)
                {
                    return Result.NotFound(new Error("NotFound", $"Country '{id}' was not found"));
                }

                var duplicateName = await CountryExistsAsync(updateDto.Name);
                if (duplicateName)
                {
                    return Result.Failure(new Error("Conflict", $"Conflict with name '{updateDto.Name}' already exists."));
                }
                country.Name = updateDto.Name;
                country.ShortName = updateDto.ShortName;
                context.Countries.Update(country);
                await context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure();
            }
            
        }

        public async Task<Result> DeleteCountryAsync(int id)
        {
            try
            {
                var country = await context.Countries.FindAsync(id);
                if (country is null)
                {
                    return Result.NotFound(new Error("NotFound", $"Country '{id}' was not found."));
                }
                context.Countries.Remove(country);
                await context.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception)
            {
                return Result.Failure();
            }
            
        }

        public async Task<bool> CountryExistsAsync(int id)
        {
            return await context.Countries.AnyAsync(e => e.CountryId == id);
        }

        public async Task<bool> CountryExistsAsync(string name)
        {
            return await context.Countries.AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim());
        }
    }
}
