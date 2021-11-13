// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProviderServiceController.cs
// *
// * Description: Implements the ProviderService controller for the DataCenterConsole app.
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
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.ProviderServiceRepository;
using ChocAn.DataCenterConsole.Models;
using ChocAn.DataCenterConsole.Actions;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class ProviderServiceController : Controller
    {
        private readonly ILogger<ProviderServiceController> logger;
        private readonly IProviderServiceRepository repository;
        private readonly IMapper mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        public ProviderServiceController(
            ILogger<ProviderServiceController> logger,
            IProviderServiceRepository repository,
            IMapper mapper)
        {
            this.logger = logger;
            this.repository = repository;
            this.mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string find)
        {
            var action = new IndexAction<Controller, ProviderService, ProviderServiceIndexViewModel>(this, repository, find);
            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailsAsync(decimal id)
        {
            var action = new DetailsAction<Controller, ProviderService, ProviderServiceDetailsViewModel>(this, id, repository, mapper);
            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProviderServiceCreateViewModel viewModel)
        {
            var action = new CreateAction<Controller, ProviderService, ProviderServiceCreateViewModel>(this,
                repository, viewModel, mapper, nameof(Index));
            return await action.ActionResult();
        }

        // GET: ProviderServiceController/Edit/5
        public async Task<IActionResult> EditAsync(int id)
        {
            var action = new EditAction<Controller, ProviderService, ProviderServiceEditViewModel>(this,
                id, repository, mapper);
            return await action.ActionResult();
        }

        // POST: ProviderServiceController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProviderServiceEditViewModel viewModel)
        {
            var action = new PostEditAction<Controller, ProviderService, ProviderServiceEditViewModel>(this,
                repository, viewModel, mapper, nameof(Index));
            return await action.ActionResult();
        }

        // POST: ProviderServiceController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(decimal id)
        {
            await repository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
