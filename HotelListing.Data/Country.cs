namespace HotelListing.API.Data
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        // para especificar la relacion one-to-many ( un pais muchos hoteles )
        public virtual IList<Hotel> Hotels { get; set; } // 1 country tiene varios hoteles
    }
}