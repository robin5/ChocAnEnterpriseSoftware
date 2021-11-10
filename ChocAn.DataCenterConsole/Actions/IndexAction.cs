using ChocAn.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChocAn.DataCenterConsole.Models;

namespace ChocAn.DataCenterConsole.Actions
{
    public class IndexAction<TController, TModel, TViewModel>
        where TModel : class
        where TViewModel : FindViewModel<TModel>, new()
        where TController : Controller
    {
        private readonly IGenericRepository<TModel> repository;
        private readonly TController controller;
        private readonly string find;
        public IndexAction(TController controller, IGenericRepository<TModel> repository, string find)
        {
            this.controller = controller;
            this.repository = repository;
            this.find = find;
        }
        public async Task<IActionResult> ActionResult()
        {
            List<TModel> entities = new List<TModel>();
            decimal id;

            if (decimal.TryParse(find, out id))
            {
                var provider = await repository.GetAsync(id);
                if (null != provider)
                {
                    entities.Add(provider);
                }
            }
            else
            {
                await foreach (TModel provider in repository.FindAllByNameAsync(find))
                {
                    entities.Add(provider);
                }
            }

            var vm = new TViewModel
            {
                Find = find,
                Items = entities
            };

            return controller.View(vm);
        }
    }
}
