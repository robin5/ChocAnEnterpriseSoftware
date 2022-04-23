// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: TransactionsController.cs
// *
// * Description: Implements the Transaction controller for the TransactionService API.
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ChocAn.Data;
using ChocAn.Repository;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.TransactionServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> logger;
        private readonly IRepository<Transaction> repository;
        private readonly PagingOptions defaultPagingOptions;
        public TransactionController(
            ILogger<TransactionController> logger,
            IRepository<Transaction> repository,
            IOptions<PagingOptions> defaultPagingOptions)
        {
            this.logger = logger;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
        }

        /// <summary>
        /// Retrieves all transactions from Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Transaction> sortOptions,
            [FromQuery] SearchOptions<Transaction> searchOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<Transaction> transactions = new();
                await foreach (Transaction transaction in repository.GetAllAsync(pagingOptions, sortOptions, searchOptions))
                {
                    transactions.Add(transaction);
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Retrieves an individual transaction from the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var transaction = await repository.GetAsync(id);
                if (null == transaction)
                {
                    return NotFound();
                }

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new transaction into the Transaction repository.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost(Name = nameof(PostAsync))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] Transaction transaction)
        {
            try
            {
                // Verify transaction's ID = 0 to enforce good behavior
                if (transaction.Id != 0)
                    return BadRequest();

                var result = await repository.AddAsync(new Transaction
                {
                    ProviderId = transaction.ProviderId,
                    MemberId = transaction.MemberId,
                    ProductId = transaction.ProductId,
                    ProductCost = transaction.ProductCost,
                    ServiceDate = transaction.ServiceDate,
                    ServiceComment = transaction.ServiceComment,
                    Created = DateTime.UtcNow
                });

                if (null == result)
                    return BadRequest();

                return Created("", result);
            }
            catch (DbUpdateException ex)
            {
                logger?.LogError(ex, nameof(PostAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {

                logger?.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }


        /// <summary>
        /// Updates a transaction in the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <param name="transactionResource">Transaction updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Transaction transaction)
        {
            try
            {
                // Verify product's ID and the ID of the endpoint are the same
                if (transaction.Id != id)
                    return BadRequest();

                var oldTransaction = await repository.GetAsync(transaction.Id);
                if (null == oldTransaction)
                    return NotFound();

                var numChanged = await repository.UpdateAsync(new Transaction
                {
                    Id = transaction.Id,
                    ProviderId = transaction.ProviderId,
                    MemberId = transaction.MemberId,
                    ProductId = transaction.ProductId,
                    ProductCost = transaction.ProductCost,
                    ServiceDate = transaction.ServiceDate,
                    ServiceComment = transaction.ServiceComment,
                    Created = oldTransaction.Created
                });

                if (numChanged > 0)
                    return Ok();
                else
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger?.LogError(ex, nameof(PutAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(PutAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Deletes a transaction from the Transaction respoitory.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var transaction = await repository.DeleteAsync(id);
                if (null == transaction)
                {
                    return NotFound();
                }
                return Ok(transaction);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
