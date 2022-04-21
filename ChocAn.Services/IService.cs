
namespace ChocAn.Services
{
    public interface IService<TResource, TModel> where TResource : class
    {
        IService<TResource, TModel> Paginate(int offset, int limit);
        IService<TResource, TModel> AddSearch(string value);
        IService<TResource, TModel> OrderBy(string value);
        Task<(bool isSuccess, TModel? result, string? errorMessage)> GetAsync(int id);
        Task<(bool isSuccess, IEnumerable<TModel>? result, string? errorMessage)> GetAllAsync();
        Task<(bool isSuccess, TResource? result, string? errorMessage)> CreateAsync(TResource entity);
        Task<(bool isSuccess, string? errorMessage)> UpdateAsync(int id, TResource entity);
        Task<(bool isSuccess, TModel? result, string? errorMessage)> DeleteAsync(int id);
    }
}
