// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: CreateAction.cs
// *
// * Description: Implements a generic HttpGet Create action with Controller, Model, 
// *              and ViewModel parameter types
// *
// **********************************************************************************
// * Author: Robin Murray
// **********************************************************************************
// *
// * Granting License: The MIT License (MIT)
// * 
// *   Permission is hereby granted, free of charge, to any person obtaining a copy
// *   of this software and associated documentation files (the "Software"), to deal
// *   in the Software without restriction, including without limitation the rights
// *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// *   copies of the Software, and to permit persons to whom the Software is
// *   furnished to do so, subject to the following conditions:
// *   The above copyright notice and this permission notice shall be included in
// *   all copies or substantial portions of the Software.
// *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// *   THE SOFTWARE.
// * 
// **********************************************************************************

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.GenericRepository;
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
