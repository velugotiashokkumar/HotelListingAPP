using System.ComponentModel.DataAnnotations;

namespace HotelListingApi.DTOs.Hotel
{
    public class UpdateHotelDto : CreateHotelDto
    {
        [Required]
        public int Id { get; set; }
    }
}
