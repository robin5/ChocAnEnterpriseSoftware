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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ChocAn.ProviderRepository;
using ChocAn.DataCenterConsole.Models;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class ProviderController : Controller
    {
        private readonly ILogger<ProviderController> logger;
        private readonly IProviderRepository providerService;

        public ProviderController(ILogger<ProviderController> logger,
            IProviderRepository providerService)
        {
            this.logger = logger;
            this.providerService = providerService;
        }

        // GET: ProviderController
        public async Task<IActionResult> Index()
        {
            List<Provider> listProviders = new List<Provider>();
            await foreach (Provider member in providerService.GetAllAsync())
            {
                listProviders.Add(member);
            }

            var vm = new ProviderIndexViewModel
            {
                Providers = listProviders
            };

            return View(vm);
        }

        // GET: ProviderController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProviderController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProviderController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProviderController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ProviderController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ProviderController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProviderController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
