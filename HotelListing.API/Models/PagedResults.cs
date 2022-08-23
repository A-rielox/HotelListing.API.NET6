namespace HotelListing.API.Models
{
    public class PagedResults<T>
    {
        public int TotalCount { get; set; } // total de items
        public int PageNumber { get; set; } // pagina actual
        public int RecordNumber { get; set; } // cantidad de items enviados
        public List<T> Items { get; set; } // los items enviados
    }
}
