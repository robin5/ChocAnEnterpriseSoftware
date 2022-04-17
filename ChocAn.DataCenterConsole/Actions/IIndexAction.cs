using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IIndexAction<TModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TModel> Service { get; set; }
        public Task<IActionResult> ActionResult(string find);
    }
}
