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

using System.Net;
using Microsoft.AspNetCore.Mvc;
using ChocAn.Data;
using ChocAn.Services;

namespace ChocAn.TerminalServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class TerminalController : ControllerBase
    {
        #region 500 level status returns
        public const int ServiceUnavailable = (int)HttpStatusCode.ServiceUnavailable;
        public const int InternalServerError = (int)HttpStatusCode.InternalServerError;
        #endregion

        #region Error messages
        public const string MemberErrorMessage = $"Error while processing request for {nameof(Member)}";
        public const string MemberExceptionMessage = $"Exception while processing request for {nameof(Member)}";
        public const string ProviderErrorMessage = $"Error while processing request for {nameof(Provider)}";
        public const string ProviderExceptionMessage = $"Exception while processing request for {nameof(Provider)}";
        public const string ProductErrorMessage = $"Error while processing request for {nameof(Product)}";
        public const string ProductExceptionMessage = $"Exception while processing request for {nameof(Product)}";
        public const string TransactionErrorMessage = $"Error while processing transaction for {nameof(Transaction)}";
        public const string TransactionExceptionMessage = $"Exception while processing request for {nameof(Transaction)}";
        public const string TransactionProviderErrorMessage = $"Error while retrieving provider for {nameof(Transaction)}";
        public const string TransactionMemberErrorMessage = $"Error while retrieving member for {nameof(Transaction)}";
        public const string TransactionProductErrorMessage = $"Error while retrieving member product for {nameof(Transaction)}";
        public const string TransactionProviderNotFoundMessage = $"Provider not found while processing request for {nameof(Transaction)}";
        public const string TransactionMemberNotFoundMessage = $"Member not found while processing request for {nameof(Transaction)}";
        public const string TransactionProductNotFoundMessage = $"Product not found while processing request for {nameof(Transaction)}";
        public const string TransactionNotFoundMessage = $"Transaction not found while processing request for {nameof(Transaction)}";
        public const string TransactionCostNotValid = $"Transaction does not agree with product cost {nameof(Transaction)}";
        #endregion

        // Private members
        private readonly ILogger<TerminalController> logger;
        private readonly IService<Member> memberService;
        private readonly IService<Provider> providerService;
        private readonly IService<Product> productService;
        private readonly IService<Transaction> transactionService;

        /// <summary>
        /// TerminalController constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="memberService">Member service</param>
        /// <param name="providerService">Provider service</param>
        /// <param name="productService">Product service</param>
        /// <param name="transactionService">Transaction service</param>
        public TerminalController(
            ILogger<TerminalController> logger,
            IService<Member> memberService,
            IService<Provider> providerService,
            IService<Product> productService,
            IService<Transaction> transactionService
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
        [ProducesResponseType(500)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> Member(int id)
        {
            try
            {
                var (success, member, error) = await memberService.GetAsync(id);
                if (success)
                {
                    if (member == null)
                        return NotFound();
                    else
                        return Ok(new
                        {
                            member.Id,
                            member.Status
                        });
                }

                logger?.LogError(MemberErrorMessage, error);
                return StatusCode(ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, MemberExceptionMessage, id);
                return StatusCode(InternalServerError);
            }
        }

        /// <summary>
        /// Verifis existance of a provider
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns></returns>
        [HttpGet("provider/{id}", Name = nameof(Provider))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> Provider(int id)
        {
            try
            {
                var (success, provider, error) = await providerService.GetAsync(id);
                if (success)
                {
                    if (provider == null)
                        return NotFound();
                    else
                        return Ok(new
                        {
                            provider.Id,
                            provider.Name
                        });
                }

                logger?.LogError(ProviderErrorMessage, error);
                return StatusCode(ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ProviderExceptionMessage, id);
                return StatusCode(InternalServerError);
            }
        }

        /// <summary>
        /// Verifies existance of a product and returns the
        /// products's id, name, and cost.
        /// </summary>
        /// <param name="id">Provider service's identification code</param>
        /// <returns></returns>
        [HttpGet("product/{id}", Name = nameof(Product))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> Product(int id)
        {
            try
            {
                var (success, product, error) = await productService.GetAsync(id);
                if (success)
                {
                    if (product == null)
                        return NotFound();
                    else
                        return Ok(new
                        {
                            product.Id,
                            product.Name,
                            product.Cost
                        });
                }

                logger?.LogError(ProductErrorMessage, error);
                return StatusCode(ServiceUnavailable);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ProductExceptionMessage, id);
                return StatusCode(InternalServerError);
            }
        }

        /// <summary>
        /// Inserts a transaction
        /// </summary>
        /// <param name="transaction">Transaction's values</param>
        /// <returns></returns>
        [HttpPost("transaction", Name = nameof(Transaction))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(503)]
        public async Task<IActionResult> Transaction([FromBody] Transaction transaction)
        {
            try
            {
                // Verify provider exists
                var (providerSuccess, provider, providerError) = await providerService.GetAsync(transaction.ProviderId);
                if (!providerSuccess)
                {
                    logger?.LogError(TransactionProviderErrorMessage, providerError);
                    return StatusCode(ServiceUnavailable);
                }
                else if (provider == null)
                {
                    logger?.LogInformation(TransactionProviderNotFoundMessage);
                    return BadRequest();
                }

                // Verify member exists
                var (memberSuccess, member, memberError) = await memberService.GetAsync(transaction.MemberId);
                if (!memberSuccess)
                {
                    logger?.LogError(TransactionMemberErrorMessage, memberError);
                    return StatusCode(ServiceUnavailable);
                }
                else if (member == null)
                {
                    logger?.LogError(TransactionMemberNotFoundMessage);
                    return BadRequest();
                }

                // Verify product exists
                var (productSuccess, product, productError) = await productService.GetAsync(transaction.ProductId);
                if (!productSuccess)
                {
                    logger?.LogError(TransactionProductErrorMessage, productError);
                    return StatusCode(ServiceUnavailable);
                }
                else if (product == null)
                {
                    logger?.LogError(TransactionProductNotFoundMessage);
                    return BadRequest();
                }

                if (transaction.ProductCost != product.Cost)
                {
                    logger?.LogError(TransactionCostNotValid);
                    return BadRequest();
                }

                // Execute transaction
                var (transactionSuccess, result, transactionError) = await transactionService.CreateAsync(new Transaction
                {
                    ProviderId = provider.Id,
                    MemberId = member.Id,
                    ProductId = product.Id,
                    ProductCost = product.Cost,
                    ServiceDate = transaction.ServiceDate,
                    ServiceComment = transaction.ServiceComment
                });

                if (!transactionSuccess)
                {
                    logger?.LogError(TransactionErrorMessage, transactionError);
                    return StatusCode(ServiceUnavailable);
                }

                if (result == null)
                {
                    logger?.LogError(TransactionNotFoundMessage);
                    return NotFound();
                }

                // Report transaction accepted
                return Created(string.Empty, new { result.Id });
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, TransactionExceptionMessage, transaction);
                return StatusCode(InternalServerError);
            }
        }
    }
}
