using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Repository;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DeleteAction<TModel> : IDeleteAction<TModel>
        where TModel : class
    {
        public Controller Controller { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public async Task<IActionResult> ActionResult(int id, string indexAction = null)
        {
            await Repository.DeleteAsync(id);
            return Controller.RedirectToAction(indexAction);
        }
    }
}
