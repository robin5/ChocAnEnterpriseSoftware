using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.GenericRepository;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IDeleteAction<TModel>
        where TModel : class
    {
        public Controller Controller { get; set; }
        public IGenericRepository<TModel> Repository { get; set; }
        public Task<IActionResult> ActionResult(int id, string indexAction = null);
    }
}
