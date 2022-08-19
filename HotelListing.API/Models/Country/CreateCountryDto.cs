using System.ComponentModel.DataAnnotations;

namespace HotelListing.API.Models.Country
{
    // Modelo para ser ocupado en la creacion de countries
    public class CreateCountryDto
    {
        [Required]
        public string Name { get; set; }
        public string ShortName { get; set; }
    }
}
