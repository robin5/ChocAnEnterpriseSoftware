using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.Services
{
    public interface IService<T> where T : class
    {
        IService<T> Paginate(int offset, int limit);
        IService<T> AddSearch(string value);
        IService<T> OrderBy(string value);
        Task<(bool isSuccess, T? result, string? errorMessage)> GetAsync(int id);
        Task<(bool isSuccess, IEnumerable<T>? result, string? errorMessage)> GetAllAsync();
        Task<(bool isSuccess, T? result, string? errorMessage)> CreateAsync(T entity);
    }
}
