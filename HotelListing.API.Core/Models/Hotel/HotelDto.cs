namespace HotelListing.API.Core.Models.Hotel
{
    public class HotelDto : BaseHotelDto
    {
        public int Id { get; set; }

        // UN DTO NUNCA DEBE INCLUIR UN FIELD QUE ESTE DIRECTAMENTE RELACIONADO A 
        // UN MODEL TYPE como este
        //public Country Country { get; set; }
    }
}
