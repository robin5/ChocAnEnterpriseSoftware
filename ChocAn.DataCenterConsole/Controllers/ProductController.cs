// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProductController.cs
// *
// * Description: Implements the Product controller for the DataCenterConsole app.
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
using ChocAn.Data;
using ChocAn.DataCenterConsole.Models;
using ChocAn.DataCenterConsole.Actions;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class ProductController : DataCenterController<Product>
    {
        private readonly IIndexAction<Product> indexAction;
        private readonly IDetailsAction<Product> detailsAction;
        private readonly ICreateAction<Product, ProductCreateViewModel> createAction;
        private readonly IEditAction<Product, ProductEditViewModel> editAction;
        private readonly IDeleteAction<Product> deleteAction;

        /// <summary>
        /// Constructor for ProductController
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        /// <param name="indexAction"></param>
        /// <param name="detailsAction"></param>
        /// <param name="createAction"></param>
        /// <param name="editAction"></param>
        /// <param name="deleteAction"></param>
        public ProductController(
            ILogger<ProductController> logger,
            IMapper mapper,
            IService<Product> service,
            IIndexAction<Product> indexAction,
            IDetailsAction<Product> detailsAction,
            ICreateAction<Product, ProductCreateViewModel> createAction,
            IEditAction<Product, ProductEditViewModel> editAction,
            IDeleteAction<Product> deleteAction)
            : base(logger, mapper, service)
        {
            this.indexAction = indexAction;
            this.detailsAction = detailsAction;
            this.createAction = createAction;
            this.editAction = editAction;
            this.deleteAction = deleteAction;
        }

        /// <summary>
        /// HttpGet endpoint for product/
        /// </summary>
        /// <returns>Index view</returns>
        [HttpGet]
        public async Task<IActionResult> Index(string find) =>
            await indexAction.ActionResult(this, find);

        /// <summary>
        /// HttpGet endpoint for product/details/{id}
        /// </summary>
        /// <param name="id">ID of product</param>
        /// <returns>Details view</returns>
        [HttpGet]
        public async Task<IActionResult> DetailsAsync(int id) =>
            await detailsAction.ActionResult(this, id);

        /// <summary>
        /// HttpGet endpoint for product/create
        /// </summary>
        /// <returns>Create view</returns>
        [HttpGet]
        public ActionResult Create() => View(new ProductCreateViewModel());

        /// <summary>
        /// HttpPost endpoint for product/create/{form-data}
        /// </summary>
        /// <param name="viewModel">Create form data</param>
        /// <returns>Create view or redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(ProductCreateViewModel viewModel) =>
            await createAction.ActionResult(this, viewModel);

        /// <summary>
        /// HttpGet endpoint for product/edit/{id}
        /// </summary>
        /// <param name="id">ID of product</param>
        /// <returns>Edit view</returns>
        [HttpGet]
        public async Task<IActionResult> EditAsync(int id) =>
            await editAction.ActionResult(this, id);

        /// <summary>
        /// HttpPost endpoint for product/edit/{form-data}
        /// </summary>
        /// <param name="viewModel">Edited form data</param>
        /// <returns>Edit view or redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(ProductEditViewModel viewModel) =>
            await editAction.ActionResult(this, viewModel.Id, viewModel);

        /// <summary>
        /// HttpPost endpoint for product/delete/{id}
        /// </summary>
        /// <param name="id">ID of product</param>
        /// <returns>Redirects to Index view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DCDeleteAsync(int id) =>
            await deleteAction.ActionResult(this, id);
    }
}
