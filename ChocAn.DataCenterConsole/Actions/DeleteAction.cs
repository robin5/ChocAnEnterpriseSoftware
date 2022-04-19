using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DeleteAction<TResource, TModel> : IDeleteAction<TResource, TModel>
        where TResource : class
        where TModel : class
    {
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TResource, TModel> Service { get; set; }
        public IMapper Mapper { get; set; }
        public async Task<IActionResult> ActionResult(int id, string indexAction = null)
        {
            await Service.DeleteAsync(id);
            return Controller.RedirectToAction(indexAction);
        }
    }
}
