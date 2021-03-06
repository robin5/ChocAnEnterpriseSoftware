// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: CreateAction.cs
// *
// * Description: Implements a generic HttpGet Index action with Controller, Model, 
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
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChocAn.DataCenterConsole.Models;
using ChocAn.DataCenterConsole.Controllers;

namespace ChocAn.DataCenterConsole.Actions
{
    public class IndexAction<TModel, TViewModel> : IIndexAction<TModel>
        where TModel : class
        where TViewModel : IndexViewModel<TModel>, new()
    {
        private const string LogExceptionTemplate = "DetailsAction: {ex}";
        private const string LogErrorTemplate = "DetailsAction: {error}";

        #region 500 level status returns
        //public const int ServiceUnavailable = (int)HttpStatusCode.ServiceUnavailable;
        //public const int InternalServerError = (int)HttpStatusCode.InternalServerError;
        #endregion

        public const string MemberErrorMessage = "Error while processing request for api/member/{id}";
        public async Task<IActionResult> ActionResult(
            DataCenterController<TModel> controller,
            string find)
        {
            List<TModel> items = new();

            string error = null;

            try
            {
                // Short circuit test for default index view with nothing in the find box
                if (string.IsNullOrWhiteSpace(find))
                {
                    return controller.View(new TViewModel
                    {
                        Find = "",
                        Items = items
                    });
                }

                // Parse find for a model id or name
                if (int.TryParse(find, out int id))
                {
                    var (success, model, errorMessage) = await controller.Service.GetAsync(id);
                    if (success)
                    {
                        if (model != null)
                        {
                            items.Add(model);
                        }
                    }
                    else
                    {
                        error = errorMessage;
                        controller.Logger?.LogError(LogErrorTemplate, error);
                    }
                }
                else
                {
                    var (success, models, errorMessage) = await controller.Service
                        .AddSearch($"name eq {find}")
                        .OrderBy("name")
                        .Paginate(0, 25)
                        .GetAllAsync();

                    if (success)
                    {
                        foreach (var model in models)
                        {
                            items.Add(model);
                        }
                    }
                    else
                    {
                        error = errorMessage;
                        controller.Logger?.LogError(LogErrorTemplate, error);
                    }
                }
            }
            catch (Exception ex)
            {
                controller.Logger?.LogError(LogExceptionTemplate, ex);
                error = ex.Message;
            }

            if (null != error)
                controller.ModelState.AddModelError("Error", error);

            return controller.View(new TViewModel
            {
                Find = find,
                Items = items
            });
        }
    }
}
