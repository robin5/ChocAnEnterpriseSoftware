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
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProviderServiceRepository;
using ChocAn.TransactionRepository;
using ChocAn.ProviderTerminal.Api.Resources;

namespace ChocAn.ProviderTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TerminalController : ControllerBase
    {
        private readonly ILogger<TerminalController> logger;
        private readonly IProviderRepository providerService;
        private readonly IMemberRepository memberService;
        private readonly IProviderServiceRepository providerServiceService;
        private readonly ITransactionRepository transactionService;
        public TerminalController(
            ILogger<TerminalController> logger,
            IProviderRepository providerService,
            IMemberRepository memberService,
            IProviderServiceRepository providerServiceService,
            ITransactionRepository transactionService)
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
        /// <param name="id">Provider's identification number</param>
        /// <returns></returns>
        [HttpGet("provider/{number}", Name = nameof(TerminalProvider))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalProvider(decimal id)
        {
            var provider = await providerService.GetAsync(id);
            if (null == provider)
            {
                return NotFound();
            }

            return Ok(new ProviderResource
            {
                Id = provider.Id
            });
        }

        /// <summary>
        /// Verifies existance of a member and returns member status.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns></returns>
        [HttpGet("member/{number}", Name = nameof(TerminalMember))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalMember(decimal id)
        {
            var member = await memberService.GetAsync(id);
            if (null == member)
            {
                return NotFound();
            }

            return Ok(new MemberResource
            {
                Id = member.Id,
                Status = member.Status
            });
        }

        /// <summary>
        /// Verifies existance of a provider service and returns the
        /// service's code, name, and cost.
        /// </summary>
        /// <param name="id">Provider service's identification code</param>
        /// <returns></returns>
        [HttpGet("service/{code}", Name = nameof(TerminalProviderService))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> TerminalProviderService(decimal id)
        {
            var providerService = await providerServiceService.GetAsync(id);
            if (null == providerService)
            {
                return NotFound();
            }

            return Ok(new ProviderServiceResource
            {
                Id = providerService.Id,
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
            var provider = await providerService.GetAsync(terminalTransaction.ProviderId);
            var member = await memberService.GetAsync(terminalTransaction.MemberId);

            if ((null == provider) || (null == member))
            {
                return BadRequest();
            }

            var transaction = new Transaction
            {
                ProviderId = provider.Id,
                MemberId = member.Id,
                ServiceDate = terminalTransaction.ServiceDate,
                ServiceCode = terminalTransaction.ServiceId,
                ServiceComment = terminalTransaction.ServiceComment
            };

            await transactionService.AddAsync(transaction);

            // Tell terminal transaction was accepted
            return Created("", new TransactionResource { Status = "accepted"});
        }
    }
}
