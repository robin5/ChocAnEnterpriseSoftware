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
using ChocAn.Repository;

namespace ChocAn.ProviderTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TerminalController : ControllerBase
    {
        private readonly ILogger<TerminalController> logger;
        private readonly IRepository<Member> memberRepository;
        private readonly IRepository<Provider> providerRepository;
        private readonly IRepository<ProviderService> providerServiceRepository;
        private readonly ITransactionRepository transactionRepository;
        public TerminalController(
            ILogger<TerminalController> logger,
            IRepository<Member> memberRepository,
            IRepository<Provider> providerRepository,
            IRepository<ProviderService> providerServiceRepsoitory,
            ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.memberRepository = memberRepository;
            this.providerRepository = providerRepository;
            this.providerServiceRepository = providerServiceRepsoitory;
            this.transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Verifis existance of a provider
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns></returns>
        [HttpGet("provider/{id}", Name = nameof(Provider))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Provider(int id)
        {
            var provider = await providerRepository.GetAsync(id);
            if (null == provider)
            {
                return NotFound();
            }

            return Ok(new ProviderResource
            {
                Id = provider.Id,
                Name = provider.Name
            });
        }

        /// <summary>
        /// Verifies existance of a member and returns member status.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns></returns>
        [HttpGet("member/{id}", Name = nameof(Member))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Member(int id)
        {
            var member = await memberRepository.GetAsync(id);
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
        [HttpGet("service/{id}", Name = nameof(ProviderService))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ProviderService(int id)
        {
            var providerService = await providerServiceRepository.GetAsync(id);
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
        public async Task<IActionResult> Transaction([FromBody] TransactionResource transactionResource)
        {
            var provider = await providerRepository.GetAsync(transactionResource.ProviderId);
            var member = await memberRepository.GetAsync(transactionResource.MemberId);
            var providerService = await providerServiceRepository.GetAsync(transactionResource.ServiceId);

            if ((null == provider) || (null == member) || (null == providerService))
            {
                return BadRequest();
            }

            var transaction = new Transaction
            {
                ProviderId = provider.Id,
                MemberId = member.Id,
                ServiceId = providerService.Id,
                ServiceDate = transactionResource.ServiceDate,
                ServiceComment = transactionResource.ServiceComment
            };

            await transactionRepository.AddAsync(transaction);

            // Tell terminal transaction was accepted
            return Created("", transactionResource);
        }
    }
}
