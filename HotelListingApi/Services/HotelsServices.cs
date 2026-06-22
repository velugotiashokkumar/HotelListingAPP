using AutoMapper;
using AutoMapper.QueryableExtensions;
using HotelListingApi.Contracts;
using HotelListingApi.Data;
using HotelListingApi.Results;
using HotelListingApi.DTOs.Country;
using HotelListingApi.DTOs.Hotel;
using Microsoft.EntityFrameworkCore;
using HotelListingApi.Constants;

namespace HotelListingApi.Services
{
    public class HotelsServices(HotelListingDbContext context, ICountriesServices countriesServices, IMapper mapper) : IHotelsServices
    {
        public async Task<Result<IEnumerable<GetHotelDto>>> GetHotelsAsync()
        {
            var hotels = await context.Hotels
            .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
            .ToListAsync();

            return Result<IEnumerable<GetHotelDto>>.Success(hotels);
        }

        public async Task<Result<GetHotelDto>> GetHotelAsync(int id)
        {
            //SELECT h.Id,
            // h.Name,
            // h.Address,
            // h.Rating,
            // c.Name FROM Hotels h
            // LEFT JOIN Countries c ON h.CountryId = c.CountryId
            // WHERE h.Id = @id

            //Without Automapper
            //var hotel = await context.Hotels
            //    .Where(j => j.Id == id)
            //    .Include(q => q.Country)
            //    .Select(j => new GetHotelDto(j.Id, j.Name, j.Address, j.Rating, j.CountryId, j.Country!.Name))
            //    .FirstOrDefaultAsync();

            //With Automapper
            var hotel = await context.Hotels
                .Where(j => j.Id == id)
                .Include(q => q.Country)
                .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (hotel is null)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));
            }

            return Result<GetHotelDto>.Success(hotel);

        }

        public async Task<Result<GetHotelDto>> CreateHotelAsync(CreateHotelDto hotelDto)
        {
            var countryExists = await countriesServices.CountryExistsAsync(hotelDto.CountryId);
            if (!countryExists)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.NotFound, $"Country '{hotelDto.CountryId}' was not found."));
            }

            var duplicate = await HotelExistsAsync(hotelDto.Name, hotelDto.CountryId);
            if (duplicate)
            {
                return Result<GetHotelDto>.Failure(new Error(ErrorCodes.Conflict, $"Hotel '{hotelDto.Name}' already exists in the selected country."));
            }
            //  Whithout Automapper
            //var hotel = new Hotel
            //{
            //    Name = hotelDto.Name,
            //    Address = hotelDto.Address,
            //    Rating = hotelDto.Rating,
            //    CountryId = hotelDto.CountryId
            //};

            // With Automapper
            var hotel = mapper.Map<Hotel>(hotelDto);

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();

            // Without Aoutomapper
            //return new GetHotelDto(
            //    hotel.Id,
            //    hotel.Name,
            //    hotel.Address,
            //    hotel.Rating,
            //    hotel.CountryId,
            //    string.Empty // Assuming you want to return an empty string for the country name since it's not available at this point
            //);

            // With Automapper
            //return mapper.Map<GetHotelDto>(hotel);

            var dto = await context.Hotels
                .Where(h => h.Id == hotel.Id)
                .ProjectTo<GetHotelDto>(mapper.ConfigurationProvider)
                .FirstAsync();

            return Result<GetHotelDto>.Success(dto);
        }

        public async Task<Result> UpdateHotelAsync(int id, UpdateHotelDto updateDto)
        {

            if (id != updateDto.Id)
            {
                return Result.BadRequest(new Error(ErrorCodes.Validation, "Id route value does not match payload Id"));
            }

            var hotel = await context.Hotels.FindAsync(id);
            if (hotel is null)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id} was not found.'"));
            }

            var countryExists = await context.Countries.AnyAsync(c => c.CountryId == updateDto.CountryId);
            if (!countryExists)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Country '{updateDto.CountryId}' was not found"));
            }

            hotel.Name = updateDto.Name;
            hotel.Address = updateDto.Address;
            hotel.Rating = updateDto.Rating;
            hotel.CountryId = updateDto.CountryId;

            context.Hotels.Update(hotel);
            await context.SaveChangesAsync();

            return Result.Success();
            //var hotel = await context.Hotels.FindAsync(id) ?? throw new KeyNotFoundException("Hotel not found");
            //// Without Automapper
            ////hotel.Name = updateDto.Name;
            ////hotel.Address = updateDto.Address;
            ////hotel.Rating = updateDto.Rating;
            ////hotel.CountryId = updateDto.CountryId;

            //// With Automapper
            //mapper.Map<Hotel>(updateDto);

            //context.Hotels.Update(hotel);
            //await context.SaveChangesAsync();


        }

        public async Task<Result> DeleteHotelAsync(int id)
        {
            var affected = await context.Hotels
                .Where(h => h.Id == id)
                .ExecuteDeleteAsync();

            if (affected == 0)
            {
                return Result.NotFound(new Error(ErrorCodes.NotFound, $"Hotel '{id}' was not found."));
            }

            return Result.Success();
        }

        public async Task<bool> HotelExistsAsync(int id)
        {
            return await context.Hotels.AnyAsync(e => e.Id == id);
        }

        public async Task<bool> HotelExistsAsync(string name, int countryId)
        {
            return await context.Hotels
                .AnyAsync(e => e.Name.ToLower().Trim() == name.ToLower().Trim() && e.CountryId == countryId);
        }
    }
}
