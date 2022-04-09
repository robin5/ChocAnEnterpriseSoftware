using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Repository;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IIndexAction<TModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public IService<TModel> Service { get; set; }
        public Task<IActionResult> ActionResult(string find);
        public Task<IActionResult> ActionResult2(string find);
    }
}
