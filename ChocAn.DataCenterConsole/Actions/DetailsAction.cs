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
using ChocAn.Services;
using ChocAn.DataCenterConsole.Models;
using AutoMapper;

namespace ChocAn.DataCenterConsole.Actions
{
    public class DetailsAction<TModel, TViewModel> : IDetailsAction<TModel>
        where TModel : class
        where TViewModel : class, new()
    {
        public const string ErrorMessage = $"Error while processing request for {nameof(TModel)}";
        public const string NotFoundMessage = $"Item not found";
        public Controller Controller { get; set; }
        public ILogger<Controller> Logger { get; set; }
        public IMapper Mapper { get; set; }
        public IService<TModel> Service { get; set; }
        public async Task<IActionResult> ActionResult(int id)
        {
            string error = null;

            try
            {
                // Get an item from the service
                var (success, item, errorMessage) = await Service.GetAsync(id);
                if (success)
                {
                    if (null != item)
                    {
                        // Map the item into the TViewModel
                        var viewModel = Mapper.Map<TViewModel>(item);

                        // Render the view
                        return Controller.View(viewModel);
                    }
                    else
                    {
                        // Record not found error
                        Logger?.LogError(NotFoundMessage);
                        error = NotFoundMessage;
                    }
                }
                else
                {
                    // Record service error
                    Logger?.LogError(ErrorMessage, errorMessage);
                    error = ErrorMessage;
                }
            }
            catch (Exception ex)
            {
                // Record exception
                Logger?.LogError(error, ex);
                error = ex.Message;
            }

            Controller.ModelState.AddModelError("Error", error);

            // Render view with error and no item
            return Controller.View();
        }
    }
}
