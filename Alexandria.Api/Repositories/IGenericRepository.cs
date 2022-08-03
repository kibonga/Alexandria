using Alexandria.Api.Models.Response;

namespace Alexandria.Api.Repositories
{
    public interface IGenericRepository<T>
        where T : class
    {
        Task AddAsync(T entity);
        Task DeleteAsync(int id);
        Task<bool> Exists(int id);
        Task<List<T>> GetAllAsync();
        Task<VirtualizedResponse<TResult>> GetAllAsync<TResult>(QueryParameters queryParams) where TResult : class;
        Task<T> GetAsync(int? id);
        Task UpdateAsync(T entity);
    }
}
