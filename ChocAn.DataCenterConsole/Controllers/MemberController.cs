// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MemberController.cs
// *
// * Description: Implements the Member controller for the DataCenterConsole app.
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
using ChocAn.MemberRepository;
using ChocAn.DataCenterConsole.Models;
using ChocAn.DataCenterConsole.Actions;

namespace ChocAn.DataCenterConsole.Controllers
{
    public class MemberController : Controller
    {
        private readonly ILogger<MemberController> logger;
        private readonly IMemberRepository memberService;
        private readonly IMapper mapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="repository"></param>
        /// <param name="mapper"></param>
        public MemberController(
            ILogger<MemberController> logger,
            IMemberRepository repository,
            IMapper mapper)
        {
            this.logger = logger;
            this.memberService = repository;
            this.mapper = mapper;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string find)
        {
            var action = new IndexAction<Controller, Member, MemberIndexViewModel>(this, memberService, find);
            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DetailsAsync(int id)
        {
            var action = new DetailsAction<Controller, Member, MemberDetailsViewModel>(this, id, memberService, mapper);
            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(MemberCreateViewModel viewModel)
        {
            var action = new CreateAction<Controller, Member, MemberCreateViewModel>(this,
                memberService, viewModel, mapper, nameof(Index));
            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> EditAsync(int id)
        {
            var action = new EditAction<Controller, Member, MemberEditViewModel>(this,
                id, memberService, mapper);

            return await action.ActionResult();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(MemberEditViewModel viewModel)
        {
            var action = new PostEditAction<Controller, Member, MemberEditViewModel>(this,
                memberService, viewModel, mapper, nameof(Index));
            return await action.ActionResult();
        }

        // POST: MemberController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id)
        {
            await memberService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
