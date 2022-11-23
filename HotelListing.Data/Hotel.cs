using System.ComponentModel.DataAnnotations.Schema;

namespace HotelListing.API.Data
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; }

        // CountryId es una foreign key, q va a apuntar a una tabla q se llama Country
        // se ponen las 2 lineas juntas, la de abajo es la mencion a la tabla, asi se 
        // deja expresado que CountryId es una foreign-key a la tabla Country
        // el normal con MAGIC STRING "[ForeignKey("CountryId"))]" no avisa si hay
        // error xq cambie el nombre o algo asi, xeso se ocupa mejor este
        [ForeignKey(nameof(CountryId))]
        public int CountryId { get; set; } // c/hotel pertenece a un country
        public Country Country { get; set; }
    }
}
