﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Repository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public interface IEditAction<TModel, TViewModel> where TModel : class
    {
        public Controller Controller { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public IMapper Mapper { get; set; }
        public Task<IActionResult> ActionResult(int id);
        public Task<IActionResult> ActionResult(TViewModel viewModel, string indexAction = null);
    }
}
