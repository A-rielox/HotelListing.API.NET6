namespace HotelListing.API.Models.Hotel
{
    public class HotelDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }
        public int CountryId { get; set; }

        // UN DTO NUNCA DEBE INCLUIR UN FIELD QUE ESTE DIRECTAMENTE RELACIONADO A 
        // UN MODEL TYPE como este
        //public Country Country { get; set; }
    }
}
