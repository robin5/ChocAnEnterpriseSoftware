// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProviderController.cs
// *
// * Description: Implements the Provider controller for the DataCenterConsole app.
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
using ChocAn.ProviderRepository;
using ChocAn.DataCenterConsole.Models;
using ChocAn.DataCenterConsole.Actions;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class ProviderController : Controller
    {
        private readonly ILogger<ProviderController> logger;

        private readonly IIndexAction<Provider> indexAction;
        private readonly IDetailsAction<Provider> detailsAction;
        private readonly ICreateAction<Provider, ProviderCreateViewModel> createAction;
        private readonly IEditAction<Provider, ProviderEditViewModel> editAction;
        private readonly IDeleteAction<Provider> deleteAction;

        /// <summary>
        /// Constructor for ProviderController
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="indexAction"></param>
        /// <param name="detailsAction"></param>
        /// <param name="createAction"></param>
        /// <param name="editAction"></param>
        /// <param name="deleteAction"></param>
        public ProviderController(ILogger<ProviderController> logger,
            IProviderRepository repository,
            IMapper mapper,
            IIndexAction<Provider> indexAction,
            IDetailsAction<Provider> detailsAction,
            ICreateAction<Provider, ProviderCreateViewModel> createAction,
            IEditAction<Provider, ProviderEditViewModel> editAction,
            IDeleteAction<Provider> deleteAction)
        {
            this.logger = logger;

            // Configure Index action
            this.indexAction = indexAction;
            this.indexAction.Controller = this;
            this.indexAction.Repository = repository;

            // Configure Details action
            this.detailsAction = detailsAction;
            this.detailsAction.Controller = this;
            this.detailsAction.Repository = repository;
            this.detailsAction.Mapper = mapper;

            // Configure PostEdit action
            this.createAction = createAction;
            this.createAction.Controller = this;
            this.createAction.Repository = repository;
            this.createAction.Mapper = mapper;

            // Configure Edit action
            this.editAction = editAction;
            this.editAction.Controller = this;
            this.editAction.Repository = repository;
            this.editAction.Mapper = mapper;

            // Configure Delete action
            this.deleteAction = deleteAction;
            this.deleteAction.Controller = this;
            this.deleteAction.Repository = repository;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string find)
        {
            return await indexAction.ActionResult(find);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailsAsync(int id)
        {
            return await detailsAction.ActionResult(id);
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
        public async Task<IActionResult> CreateAsync(ProviderCreateViewModel viewModel)
        {
            return await createAction.ActionResult(viewModel, nameof(Index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditAsync(int id)
        {
            return await editAction.ActionResult(id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ProviderEditViewModel viewModel)
        {
            return await editAction.ActionResult(viewModel, nameof(Index));
        }

        // POST: ProviderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            return await deleteAction.ActionResult(id, nameof(Index));
        }
    }
}
