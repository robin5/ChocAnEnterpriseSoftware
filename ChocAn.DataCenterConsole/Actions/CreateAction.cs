using ChocAn.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public class CreateAction<TController, TModel, TViewModel>
        where TModel : class
        where TViewModel : class, new()
        where TController : Controller
    {
        private readonly IGenericRepository<TModel> repository;
        private readonly TController controller;
        private readonly TViewModel viewModel;
        private readonly IMapper mapper;
        private readonly string indexAction;
        public CreateAction(TController controller, IGenericRepository<TModel> repository, TViewModel viewModel, IMapper mapper, string indexAction)
        {
            this.controller = controller;
            this.repository = repository;
            this.viewModel = viewModel;
            this.mapper = mapper;
            this.indexAction = indexAction;
        }
        public async Task<IActionResult> ActionResult()
        {
            if (!controller.ModelState.IsValid)
            {
                return controller.View(viewModel);
            }

            var entity = mapper.Map<TModel>(viewModel);
            
            await repository.AddAsync(entity);

            return controller.RedirectToAction(indexAction);
        }
    }
}
