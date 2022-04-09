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

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Repository;
using ChocAn.DataCenterConsole.Models;
using ChocAn.Services;
using System.Net;
using Microsoft.Extensions.Logging;

namespace ChocAn.DataCenterConsole.Actions
{
    public class IndexAction<TModel, TViewModel> : IIndexAction<TModel>
        where TModel : class
        where TViewModel : IndexViewModel<TModel>, new()
    {
        #region 500 level status returns
        public const int ServiceUnavailable = (int)HttpStatusCode.ServiceUnavailable;
        public const int InternalServerError = (int)HttpStatusCode.InternalServerError;
        #endregion

        public const string MemberErrorMessage = "Error while processing request for api/member/{id}";
        public Controller Controller { get; set; }

        public ILogger<Controller> Logger { get; set; }
        public IRepository<TModel> Repository { get; set; }
        public IService<TModel> Service { get; set; }
        public async Task<IActionResult> ActionResult(string find)
        {
            List<TModel> entities = new();

            if (int.TryParse(find, out int id))
            {
                var provider = await Repository.GetAsync(id);
                if (null != provider)
                {
                    entities.Add(provider);
                }
            }
            else
            {
                await foreach (TModel provider in Repository.GetAllByNameAsync(find))
                {
                    entities.Add(provider);
                }
            }

            var vm = new TViewModel
            {
                Find = find,
                Items = entities
            };

            return Controller.View(vm);
        }
        public async Task<IActionResult> ActionResult2(string find)
        {
            List<TModel> items = new();

            if (int.TryParse(find, out int id))
            {
                var (success, result, error) = await Service.GetAsync(id);
                if (success)
                {
                    if (result == null)
                        return Controller.NotFound();
                    else
                    {
                        items.Add(result);
                    }
                }
                else
                {
                    Logger?.LogError(MemberErrorMessage, error);
                    return Controller.StatusCode(ServiceUnavailable);
                }
            }
            else
            {
                var (success, result, error) = await Service.GetAllByNameAsync(find);
                if(success)
                {
                    await foreach(var model in result)
                    {
                        items.Add(model);
                    }
                }
                else
                {
                    Logger?.LogError(MemberErrorMessage, error);
                    return Controller.StatusCode(ServiceUnavailable);
                }
            }

            var vm = new TViewModel
            {
                Find = find,
                Items = items
            };

            return Controller.View(vm);
        }
    }
}
