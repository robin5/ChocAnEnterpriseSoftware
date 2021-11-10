using ChocAn.DataCenterConsole.Models;
using ChocAn.GenericRepository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChocAn.MemberRepository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public class EditAction<TController, TModel, TViewModel>
        where TModel : class
        where TViewModel : class, new()
        where TController : Controller
    {
        private readonly TController controller;
        private readonly decimal id;
        private readonly IGenericRepository<TModel> repository;
        private readonly IMapper mapper;
        public EditAction(TController controller, decimal id, IGenericRepository<TModel> repository, IMapper mapper)
        {
            this.controller = controller;
            this.id = id;
            this.repository = repository;
            this.mapper = mapper;
        }
        public async Task<IActionResult> ActionResult()
        {
            var entity = await repository.GetAsync(id);
            if (null != entity)
            {
                // map VM
                var vm = mapper.Map<TViewModel>(entity);

                return controller.View(vm);
            }
            return controller.View();
        }
    }
}
