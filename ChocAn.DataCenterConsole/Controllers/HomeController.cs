// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: HomeController.cs
// *
// * Description: Implements the Home controller for the DataCenterConsole app.
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
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProviderServiceRepository;
using ChocAn.DataCenterConsole.Models;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IMemberRepository memberService;
        private readonly IProviderRepository providerService;
        private readonly IProviderServiceRepository providerServiceService;

        public HomeController(ILogger<HomeController> logger, 
            IMemberRepository memberService,
            IProviderRepository providerService,
            IProviderServiceRepository providerServiceService)
        {
            this.logger = logger;
            this.memberService = memberService;
            this.providerService = providerService;
            this.providerServiceService = providerServiceService;
        }

        public async Task<IActionResult> Index()
        {
            List<Member> listMembers = new List<Member>();
            await foreach (Member member in memberService.GetAllAsync())
            {
                listMembers.Add(member);
            }

            List<Provider> listProviders = new List<Provider>();
            await foreach (Provider provider in providerService.GetAllAsync())
            {
                listProviders.Add(provider);
            }

            List<ProviderService> listProviderServices = new List<ProviderService>();
            await foreach (ProviderService providerService in providerServiceService.GetAllAsync())
            {
                listProviderServices.Add(providerService);
            }

            var vm = new HomeIndexViewModel
            {
                Members = listMembers,
                Providers = listProviders,
                ProviderServices = listProviderServices,
            };

            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
