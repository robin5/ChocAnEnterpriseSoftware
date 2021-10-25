// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: TerminalController.cs
// *
// * Description: Main controller for the API
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
using ChocAn.MemberService;
using ChocAn.ProviderService;
using ChocAn.ProviderServiceService;
using ChocAn.TransactionService;
using ChocAn.ProviderTerminal.Api.Resources;

namespace ChocAn.ProviderTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TerminalController : ControllerBase
    {
        private readonly ILogger<TerminalController> logger;
        private readonly IProviderService providerService;
        private readonly IMemberService memberService;
        private readonly IProviderServiceService providerServiceService;
        private readonly ITransactionService transactionService;
        public TerminalController(
            ILogger<TerminalController> logger,
            IProviderService providerService,
            IMemberService memberService,
            IProviderServiceService providerServiceService,
            ITransactionService transactionService)
        {
            this.logger = logger;
            this.providerService = providerService;
            this.memberService = memberService;
            this.providerServiceService = providerServiceService;
            this.transactionService = transactionService;
        }

        // GET api/<TerminalController>/5
        /// <summary>
        /// Verifis existance of a provider
        /// </summary>
        /// <param name="number">Provider's identification number</param>
        /// <returns></returns>
        [HttpGet("provider/{number}", Name = nameof(TerminalProvider))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalProvider(decimal number)
        {
            var provider = await providerService.GetProviderByNumberAsync(number);
            if (null == provider)
            {
                return NotFound();
            }

            return Ok(new ProviderResource
            {
                Number = provider.Number
            });
        }

        /// <summary>
        /// Verifies existance of a member and returns member status.
        /// </summary>
        /// <param name="number">Member's identification number</param>
        /// <returns></returns>
        [HttpGet("member/{number}", Name = nameof(TerminalMember))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalMember(decimal number)
        {
            var member = await memberService.GetMemberByNumberAsync(number);
            if (null == member)
            {
                return NotFound();
            }

            return Ok(new MemberResource
            {
                Number = member.Number,
                Status = member.Status
            });
        }

        /// <summary>
        /// Verifies existance of a provider service and returns the
        /// service's code, name, and cost.
        /// </summary>
        /// <param name="code">Provider service's identification code</param>
        /// <returns></returns>
        [HttpGet("service/{code}", Name = nameof(TerminalProviderService))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalProviderService(decimal code)
        {
            var providerService = await providerServiceService.GetByCodeAsync(code);
            if (null == providerService)
            {
                return NotFound();
            }

            return Ok(new ProviderServiceResource
            {
                Code = providerService.Code,
                Name = providerService.Name,
                Cost = providerService.Cost
            });
        }

        // POST api/<Transaction>
        [HttpPost("transaction", Name = nameof(Transaction))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Transaction([FromBody] TransactionResource terminalTransaction)
        {
            var provider = await providerService.GetProviderByNumberAsync(terminalTransaction.ProviderNumber);
            var member = await memberService.GetMemberByNumberAsync(terminalTransaction.MemberNumber);

            if ((null == provider) || (null == member))
            {
                return BadRequest();
            }

            var transaction = new Transaction
            {
                ProviderId = provider.Id,
                MemberId = member.Id,
                ServiceDate = terminalTransaction.ServiceDate,
                ServiceCode = terminalTransaction.ServiceCode,
                ServiceComment = terminalTransaction.ServiceComment
            };

            await transactionService.AddAsync(transaction);

            // Tell terminal transaction was accepted
            return Created("", new TransactionResource { Status = "accepted"});
        }
    }
}
