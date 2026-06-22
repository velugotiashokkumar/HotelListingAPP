using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.Hotel
{
    public class CreateHotelDto
    {
        [Required]
        public required string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public required string Address { get; set; }

        [Range(1, 5)]
        public double Rating { get; set; }

        [Required]
        public int CountryId { get; set; }
    }
}
