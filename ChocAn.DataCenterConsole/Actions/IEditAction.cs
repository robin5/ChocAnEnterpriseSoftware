using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IEditAction<TResource, TModel, TViewModel>
        where TResource : class
        where TModel : class
        where TViewModel : class
    {
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TResource, TModel> Service { get; set; }
        public IMapper Mapper { get; set; }
        public Task<IActionResult> ActionResult(int id);
        public Task<IActionResult> ActionResult(int id, TViewModel viewModel, string indexAction);
    }
}
