
namespace ChocAn.Services
{
    public interface IService<TModel> where TModel : class
    {
        IService<TModel> Paginate(int offset, int limit);
        IService<TModel> AddSearch(string value);
        IService<TModel> OrderBy(string value);
        Task<(bool isSuccess, TModel? result, string? errorMessage)> GetAsync(int id);
        Task<(bool isSuccess, IEnumerable<TModel>? result, string? errorMessage)> GetAllAsync();
        Task<(bool isSuccess, TModel? result, string? errorMessage)> CreateAsync(TModel entity);
        Task<(bool isSuccess, string? errorMessage)> UpdateAsync(int id, TModel entity);
        Task<(bool isSuccess, TModel? result, string? errorMessage)> DeleteAsync(int id);
    }
}
