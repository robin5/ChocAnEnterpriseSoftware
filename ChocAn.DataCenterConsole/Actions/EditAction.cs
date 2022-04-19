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

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using ChocAn.Services;

namespace ChocAn.DataCenterConsole.Actions
{
    public class EditAction<TResource, TModel, TViewModel> : IEditAction<TResource, TModel, TViewModel>
        where TResource : class
        where TModel : class
        where TViewModel : class, new()
    {
        private const string LogExceptionTemplate = "EditAction: {ex}";
        private const string LogErrorTemplate = "EditAction: {error}";
        private const string NotFoundMessage = $"Item not found";
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IService<TResource, TModel> Service { get; set; }
        public IMapper Mapper { get; set; }
        public async Task<IActionResult> ActionResult(int id)
        {
            string error;

            try
            {
                // Get an item from the service
                var (success, model, errorMessage) = await Service.GetAsync(id);
                if (success)
                {
                    if (null != model)
                    {
                        // Map the item into the TViewModel
                        var viewModel = Mapper.Map<TViewModel>(model);

                        // Render the view
                        return Controller.View(viewModel);
                    }
                    else
                    {
                        // Record not found error
                        error = NotFoundMessage;
                        Logger?.LogError(LogErrorTemplate, error);
                    }
                }
                else
                {
                    // Record service error
                    error = errorMessage;
                    Logger?.LogError(LogErrorTemplate, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            Controller.ModelState.AddModelError("Error", error);

            // Render view with error and no item
            return Controller.View();
        }
        public async Task<IActionResult> ActionResult(int id, TViewModel viewModel, string detailsAction)
        {
            string error;

            try
            {
                if (!Controller.ModelState.IsValid)
                {
                    // Render view
                    return Controller.View(viewModel);
                }

                var resource = Mapper.Map<TResource>(viewModel);

                var (success, errorMessage) = await Service.UpdateAsync(id, resource);
                if (success)
                {
                    // TODO: Once an ID can be returned, Redirect to details page
                    return Controller.RedirectToAction(detailsAction, new { id });
                }
                else
                {
                    // Record not created error
                    error = errorMessage;
                    Logger?.LogError(LogErrorTemplate, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            // Pass error to controller via ModelState
            Controller.ModelState.AddModelError("Error", error);

            // Pass viewModel back to controller
            return Controller.View(viewModel);
        }
    }
}
