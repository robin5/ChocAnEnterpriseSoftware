// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: EditAction.cs
// *
// * Description: Implements a generic HTTPGet Edit action with Controller, Model, 
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
