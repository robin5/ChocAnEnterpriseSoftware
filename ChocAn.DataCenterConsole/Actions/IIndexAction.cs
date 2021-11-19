using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Repository;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IIndexAction<TModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public Task<IActionResult> ActionResult(string find);
    }
}
