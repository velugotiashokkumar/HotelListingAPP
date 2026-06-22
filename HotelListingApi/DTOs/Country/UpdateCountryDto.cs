using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.Country
{
    public class UpdateCountryDto : CreateCountryDto
    {
        [Required]
        public int Id { get; set; }
    }
}