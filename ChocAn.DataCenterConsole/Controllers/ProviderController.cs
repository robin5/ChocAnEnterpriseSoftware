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
using ChocAn.Services;
using ChocAn.ProviderServiceApi.Resources;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class ProviderController : DataCenterController<ProviderResource, Provider>
    {
        private readonly IIndexAction<ProviderResource, Provider> indexAction;
        private readonly IDetailsAction<ProviderResource, Provider> detailsAction;
        private readonly ICreateAction<ProviderResource, Provider, ProviderCreateViewModel> createAction;
        private readonly IEditAction<ProviderResource, Provider, ProviderEditViewModel> editAction;
        private readonly IDeleteAction<ProviderResource, Provider> deleteAction;

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
        public ProviderController(
            ILogger<ProviderController> logger,
            IMapper mapper,
            IService<ProviderResource, Provider> service,
            IIndexAction<ProviderResource, Provider> indexAction,
            IDetailsAction<ProviderResource, Provider> detailsAction,
            ICreateAction<ProviderResource, Provider, ProviderCreateViewModel> createAction,
            IEditAction<ProviderResource, Provider, ProviderEditViewModel> editAction,
            IDeleteAction<ProviderResource, Provider> deleteAction)
            : base(logger, mapper, service)
        {
            this.indexAction = indexAction;
            this.detailsAction = detailsAction;
            this.createAction = createAction;
            this.editAction = editAction;
            this.deleteAction = deleteAction;
        }

        /// <summary>
        /// HttpGet endpoint for provider/
        /// </summary>
        /// <returns>Index view</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string find) =>
            await indexAction.ActionResult(this, find);

        /// <summary>
        /// HttpGet endpoint for provider/details/{id}
        /// </summary>
        /// <param name="id">ID of provider</param>
        /// <returns>Details view</returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id) =>
            await detailsAction.ActionResult(this, id);

        /// <summary>
        /// HttpGet endpoint for provider/create
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public ActionResult Create() => View(new ProviderCreateViewModel());

        /// <summary>
        /// HttpPost endpoint for provider/create/{form-data}
        /// </summary>
        /// <param name="viewModel">Create form data</param>
        /// <returns>Create view or redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProviderCreateViewModel viewModel) =>
            await createAction.ActionResult(this, viewModel);

        /// <summary>
        /// HttpGet endpoint for provider/edit/{id}
        /// </summary>
        /// <param name="id">ID of provider</param>
        /// <returns>Edit view</returns>
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id) =>
            await editAction.ActionResult(this, id);

        /// <summary>
        /// HttpPost endpoint for provider/edit/{form-data}
        /// </summary>
        /// <param name="viewModel">Edited form data</param>
        /// <returns>Edit view or redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ProviderEditViewModel viewModel) =>
            await editAction.ActionResult(this, viewModel.Id, viewModel);

        /// <summary>
        /// HttpPost endpoint for provider/delete/{id}
        /// </summary>
        /// <param name="id">ID of provider</param>
        /// <returns>Redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DCDeleteAsync(int id) =>
            await deleteAction.ActionResult(this, id);
    }
}
