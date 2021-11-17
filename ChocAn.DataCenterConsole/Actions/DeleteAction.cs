using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.GenericRepository;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DeleteAction<TModel> : IDeleteAction<TModel>
        where TModel : class
    {
        public Controller Controller { get; set; }
        public IGenericRepository<TModel> Repository { get; set; }
        public async Task<IActionResult> ActionResult(int id, string indexAction = null)
        {
            await Repository.DeleteAsync(id);
            return Controller.RedirectToAction(indexAction);
        }
    }
}
