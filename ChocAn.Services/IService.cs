using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChocAn.Services
{
    public interface IService<T> where T : class
    {
        Task<(bool isSuccess, T? model, string? errorMessage)> GetAsync(int id);
        Task<(bool isSuccess, IAsyncEnumerable<T>? models, string? errorMessage)> GetAllByNameAsync(string find);
    }
}
