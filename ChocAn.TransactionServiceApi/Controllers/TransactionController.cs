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

using ChocAn.Repository;
using Microsoft.AspNetCore.Mvc;
using ChocAn.TransactionRepository;
using ChocAn.TransactionServiceApi.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository.Paging;
using Microsoft.Extensions.Options;

namespace ChocAn.TransactionServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> logger;
        private readonly IMapper mapper;
        private readonly IRepository<Transaction> repository;
        private readonly PagingOptions defaultPagingOptions;
        public TransactionController(
            ILogger<TransactionController> logger,
            IMapper mapper,
            IRepository<TransactionRepository.Transaction> repository,
            IOptions<PagingOptions> defaultPagingOptions)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
        }

        /// <summary>
        /// Retrieves all transactions from Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet(Name = nameof(GetAllAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagingOptions pagingOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<Transaction> transactions = new();
                await foreach (Transaction transaction in repository.GetAllAsync(pagingOptions))
                {
                    transactions.Add(transaction);
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Retrieves an individual transaction from the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpGet("{id}", Name = nameof(GetAsync))]
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
                logger.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new transaction into the Transaction repository.
        /// </summary>
        /// <param name="transactionResource"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost(Name = nameof(PostAsync))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] TransactionResource transactionResource)
        {
            try
            {
                var transaction = mapper.Map<Transaction>(transactionResource);
                await repository.AddAsync(transaction);
                return Created("", transactionResource);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, nameof(PostAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {

                logger.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }


        /// <summary>
        /// Updates a transaction in the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <param name="transactionResource">Transaction updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}", Name = nameof(PutAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] TransactionResource transactionResource)
        {
            try
            {
                var transaction = mapper.Map<Transaction>(transactionResource);
                transaction.Id = id;
                await repository.UpdateAsync(transaction);

                return Ok(transactionResource);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, nameof(PutAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PutAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Deletes a transaction from the Transaction respoitory.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpDelete("{id}", Name = nameof(DeleteAsync))]
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
                logger.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
