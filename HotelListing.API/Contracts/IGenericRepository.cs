namespace HotelListing.API.Contracts
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(); // devuelve una lista de tipo T
        Task<T> GetAsync(int? id); // devuelve un valor de clase T
        Task<T> AddAsync(T entity);
        Task DeleteAsync(int id); // no devuelve data
        Task UpdateAsync(T entity);
        Task<bool> Exist(int id);
    }
}
