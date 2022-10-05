using HotelListing.API.Core.Models;

namespace HotelListing.API.Core.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetAsync(int? id); // devuelve un valor de clase T
        Task<List<T>> GetAllAsync(); // devuelve una lista de tipo T
        Task<T> AddAsync(T entity);
        Task DeleteAsync(int id); // no devuelve data
        Task UpdateAsync(T entity);
        Task<bool> Exist(int id);

        // para el paging
        Task<PagedResults<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters);
    }
}

// es " Task " sin " ActionResult " xq este habla con la base de datos y el "Task" es
// q es una operacion asincrona q devuelve un valor, y el " ActionResult " es el
// return type for web API controller actions.