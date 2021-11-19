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
using ChocAn.Repository;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public class EditAction<TModel, TViewModel> : IEditAction<TModel, TViewModel>
        where TModel : class
        where TViewModel : class, new()
    {
        public Controller Controller { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public IMapper Mapper { get; set; }
        public async Task<IActionResult> ActionResult(int id)
        {
            var entity = await Repository.GetAsync(id);
            if (null != entity)
            {
                // map VM
                var viewModel = Mapper.Map<TViewModel>(entity);

                return Controller.View(viewModel);
            }
            return Controller.View();
        }
        public async Task<IActionResult> ActionResult(TViewModel viewModel, string indexAction = null)
        {
            if (!Controller.ModelState.IsValid)
            {
                return Controller.View(viewModel);
            }

            var entity = Mapper.Map<TModel>(viewModel);

            await Repository.UpdateAsync(entity);

            return Controller.RedirectToAction(indexAction);
        }
    }
}
