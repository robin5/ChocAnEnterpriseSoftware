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
using ChocAn.ProductRepository;
using ChocAn.TransactionRepository;
using ChocAn.ProviderTerminal.Api.Resources;
using ChocAn.Repository;
using ChocAn.Services;

namespace ChocAn.ProviderTerminal.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TerminalController : ControllerBase
    {
        private readonly ILogger<TerminalController> logger;
        private readonly IMemberService memberService;
        private readonly IProviderService providerService;
        private readonly IProductService productService;
        private readonly ITransactionService transactionService;
        public TerminalController(
            ILogger<TerminalController> logger,
            IMemberService memberService,
            IProviderService providerService,
            IProductService productService,
            ITransactionService transactionService
            )
        {
            this.logger = logger;
            this.memberService = memberService;
            this.providerService = providerService;
            this.productService = productService;
            this.transactionService = transactionService;
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
            var (success, member, error) = await memberService.GetAsync(id);
            if (success)
            {
                return Ok(new MemberResource
                {
                    Id = id,
                    Status = member.Status
                });
            }

            return NotFound(error);
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
            var (success, provider, error) = await providerService.GetAsync(id);
            if (success)
            {
                return Ok(new ProviderResource
                {
                    Id = provider.Id,
                    Name = provider.Name
                });
            }

            return NotFound(error);
        }

        /// <summary>
        /// Verifies existance of a provider service and returns the
        /// service's code, name, and cost.
        /// </summary>
        /// <param name="id">Provider service's identification code</param>
        /// <returns></returns>
        [HttpGet("service/{id}", Name = nameof(Product))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Product(int id)
        {
            var (success, product, error) = await productService.GetAsync(id);
            if (success)
            {
                return Ok(new ProductResource
                {
                    Id = product.Id,
                    Name = product.Name,
                    Cost = product.Cost
                });
            }

            return NotFound(error);
        }

        // POST api/<Transaction>
        [HttpPost("transaction", Name = nameof(Transaction))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Transaction([FromBody] TransactionResource transactionResource)
        {
            var (providerSuccess, provider, providerError) = await providerService.GetAsync(transactionResource.ProviderId);
            if (!providerSuccess || provider == null)
            {
                return BadRequest(providerError);
            }

            var (memberSuccess, member, memberError) = await memberService.GetAsync(transactionResource.MemberId);
            if (!memberSuccess || member == null)
            {
                return BadRequest(memberError);
            }

            var (productSuccess, product, productError) = await productService.GetAsync(transactionResource.ServiceId);
            if (!productSuccess || product == null)
            {
                return BadRequest(productError);
            }

            var transaction = new Transaction
            {
                ProviderId = provider.Id,
                MemberId = member.Id,
                ServiceId = product.Id,
                ServiceDate = transactionResource.ServiceDate,
                ServiceComment = transactionResource.ServiceComment
            };

            var(transactionSuccess, transactionError) = await transactionService.AddAsync(transaction);
            if (!transactionSuccess)
            {
                return BadRequest(transactionError);
            }

            // Tell terminal transaction was accepted
            return Created(string.Empty, transactionResource);
        }
    }
}
