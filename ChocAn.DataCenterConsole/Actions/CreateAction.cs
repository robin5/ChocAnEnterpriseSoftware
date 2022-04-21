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
using System.Reflection;
using System.Linq;
using ChocAn.DataCenterConsole.Controllers;

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
        public async Task<IActionResult> ActionResult(
            DataCenterController<TResource, TModel> controller,
            TViewModel viewModel)
        {
            try
            {
                if (!controller.ModelState.IsValid)
                {
                    // Render view
                    return controller.View(viewModel);
                }

                var resource = controller.Mapper.Map<TResource>(viewModel);

                var (success, model, error) = await controller.Service.CreateAsync(resource);

                var id = GetId(model);

                if (success)
                {
                    // TODO: Once an ID can be returned, Redirect to details page
                    return controller.RedirectToAction(ActionName.Details, new { id });
                }
                else
                {
                    // Record not created error
                    controller.Logger?.LogError(ErrorMessage, error);
                }
            }
            catch (Exception ex)
            {
                // Record exception
                controller.Logger?.LogError(ex, ExceptionMessage);
            }

            // Pass error to controller via ModelState
            controller.ModelState.AddModelError("Error", NotCreatedMessage);

            // Pass viewModel back to controller
            return controller.View(viewModel);
        }

        /// <summary>
        /// Searches the TModel object for an ID property and returns its value.
        /// </summary>
        /// <param name="model">model to search for an ID property</param>
        /// <returns>The value of the Id property of the TModel object</returns>
        /// <exception cref="ArgumentException">Thrown if the TModel object does not have an Id property</exception>
        static private int GetId(TModel model)
        {
            Type t = model.GetType();
            PropertyInfo[] props = t.GetProperties();

            var idProp = props.FirstOrDefault(p => p.Name == "Id");
            if ((idProp == null) || 
                (idProp.PropertyType != typeof(int)) ||
                (!idProp.CanRead))
                throw new ArgumentException("Id property missing from service response");

            var id = (int) idProp.GetValue(model);

            return id;
        }
    }
}
