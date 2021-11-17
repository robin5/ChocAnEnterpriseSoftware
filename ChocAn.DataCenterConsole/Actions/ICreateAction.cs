using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.GenericRepository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface ICreateAction<TModel, TViewModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public IGenericRepository<TModel> Repository { get; set; }
        public IMapper Mapper { get; set; }
        public Task<IActionResult> ActionResult(TViewModel viewModel, string indexAction);
    }
}
