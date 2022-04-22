// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: CreateAction.cs
// *
// * Description: Implements a generic HttpGet Details action with Controller, Model, 
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
using ChocAn.DataCenterConsole.Controllers;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DetailsAction<TModel, TViewModel> : IDetailsAction<TModel>
        where TModel : class
        where TViewModel : class, new()
    {
        private const string LogExceptionTemplate = "DetailsAction: {ex}";
        private const string LogErrorTemplate = "DetailsAction: {error}";
        private const string NotFoundMessage = $"Item not found";
        public async Task<IActionResult> ActionResult(
            DataCenterController<TModel> controller,
            int id)
        {
            string error;

            try
            {
                // Get an item from the service
                var (success, model, errorMessage) = await controller.Service.GetAsync(id);
                if (success)
                {
                    if (null != model)
                    {
                        // Map the item into the TViewModel
                        var viewModel = controller.Mapper.Map<TViewModel>(model);

                        // Render the view
                        return controller.View(viewModel);
                    }
                    else
                    {
                        // Record not found error
                        error = NotFoundMessage;
                        controller.Logger?.LogError(LogErrorTemplate, error);
                    }
                }
                else
                {
                    // Record service error
                    error = errorMessage;
                    controller.Logger?.LogError(LogErrorTemplate, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                controller.Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            controller.ModelState.AddModelError("Error", error);

            // Render view with error and no item
            return controller.View();
        }
    }
}
