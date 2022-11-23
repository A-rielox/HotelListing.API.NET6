using HotelListing.API.Core.Models.Hotel;

namespace HotelListing.API.Core.Models.Country
{
    public class CountryDto : BaseCountryDto
    {
        public int Id { get; set; }
        //en un dto nunca debo poner un model normal xeso aca abajo es a HotelDto y NO a Hotel
        public List<HotelDto> Hotels { get; set; }
    }
}
