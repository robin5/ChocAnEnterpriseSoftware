using ChocAn.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DetailsAction<TController, TModel, TViewModel>
        where TModel : class
        where TViewModel : class, new()
        where TController : Controller
    {
        private readonly IGenericRepository<TModel> repository;
        private readonly TController controller;
        private readonly decimal id;
        private readonly IMapper mapper;
        public DetailsAction(TController controller, decimal id, IGenericRepository<TModel> repository, IMapper mapper)
        {
            this.controller = controller;
            this.id = id;
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<IActionResult> ActionResult()
        {
            // Get a TModel entity from the repository
            var entity = await repository.GetAsync(id);
            if (null != entity)
            {
                // Instantiate a TViewModel from the TModel entity
                var viewModel = mapper.Map<TViewModel>(entity);
                // Render the view
                return controller.View(viewModel);
            }
            return controller.View();
        }
    }
}
