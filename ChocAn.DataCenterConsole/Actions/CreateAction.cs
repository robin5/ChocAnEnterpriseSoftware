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

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public class CreateAction<TResource, TModel, TViewModel> : ICreateAction<TResource, TModel, TViewModel>
        where TResource : class
        where TModel : class
        where TViewModel : class, new()
    {
        public const string ExceptionMessage = $"Exception while processing request for {nameof(TResource)}";
        public const string ErrorMessage = "Error while processing request for {nameof(TModel)}: {errorMesage}";
        public const string NotCreatedMessage = $"Item not created. Service not available.";
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TResource, TModel> Service { get; set; }
        public IMapper Mapper { get; set; }
        public async Task<IActionResult> ActionResult(TViewModel viewModel, string indexAction)
        {
            try
            {
                if (!Controller.ModelState.IsValid)
                {
                    // Render view
                    return Controller.View(viewModel);
                }

                var resource = Mapper.Map<TResource>(viewModel);

                var (success, model, error) = await Service.CreateAsync(resource);
                if (success)
                {
                    // TODO: Once an ID can be returned, Redirect to details page
                    return Controller.RedirectToAction(indexAction);
                }
                else
                {
                    // Record not created error
                    Logger?.LogError(ErrorMessage, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                Logger?.LogError(ex, ExceptionMessage);
            }

            // Pass error to controller via ModelState
            Controller.ModelState.AddModelError("Error", NotCreatedMessage);

            // Pass viewModel back to controller
            return Controller.View(viewModel);
        }
    }
}
