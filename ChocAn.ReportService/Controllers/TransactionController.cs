// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: TransactionController.cs
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocAn.ReportRepository;
using ChocAn.TransactionRepository;
using Microsoft.Extensions.Logging;
using ChocAn.ReportService.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChocAn.TransactionService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ILogger<TransactionController> logger;
        private readonly IMapper mapper;
        private readonly IReportRepository<MemberTransactionsReport> memberTransactionsReportRepository;
        private readonly IReportRepository<ProviderTransactionsReport> providerTransactionsReportRepository;
        private readonly IReportRepository<AccountsPayableReport> accountsPayableReportRepository;
        private readonly ITransactionRepository transactionRepository;
        public TransactionController(
            ILogger<TransactionController> logger,
            IMapper mapper,
            IReportRepository<MemberTransactionsReport> memberTransactionsReportRepository,
            IReportRepository<ProviderTransactionsReport> providerTransactionsReportRepository,
            IReportRepository<AccountsPayableReport> accountsPayableReportRepository,
            ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.memberTransactionsReportRepository = memberTransactionsReportRepository;
            this.providerTransactionsReportRepository = providerTransactionsReportRepository;
            this.accountsPayableReportRepository = accountsPayableReportRepository;
            this.transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Retrieves an individual transaction from the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpGet("member/{id}", Name = nameof(GetMemberReportAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetMemberReportAsync(int id)
        {
            try
            {
                var report = await memberTransactionsReportRepository.GetAsync(id);
                if (null == report)
                {
                    return NotFound();
                }

                var transactions = transactionRepository.GetMemberTransactionsAsync(
                    report.MemberId,
                    report.StartDate,
                    report.EndDate);

                if (null == transactions)
                {
                    return NotFound();
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetMemberReportAsync), null);
                return Problem();
            }
        }
        /// <summary>
        /// Retrieves an individual transaction from the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpGet("provider/{id}", Name = nameof(GetProviderReportAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetProviderReportAsync(int id)
        {
            try
            {
                var report = await providerTransactionsReportRepository.GetAsync(id);
                var transactions = transactionRepository.GetProviderTransactionsAsync(
                    report.ProviderId,
                    report.StartDate,
                    report.EndDate);

                if (null == transactions)
                {
                    return NotFound();
                }

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetProviderReportAsync), null);
                return Problem();
            }
        }
        /// <summary>
        /// Retrieves an individual transaction from the Transaction repository.
        /// </summary>
        /// <param name="id">Transaction's identification number</param>
        /// <returns>200 on success. 404 if transaction does not exist. 500 on exception</returns>
        [HttpGet("accountspayable/{id}", Name = nameof(GetAccountsPayableReportAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAccountsPayableReportAsync(int id)
        {
            try
            {
                var report = await accountsPayableReportRepository.GetAsync(id);
                var transactions = transactionRepository.GetAccountsPayableTransactionsAsync(
                    report.StartDate,
                    report.EndDate);


                var transactionsByProvider = new Dictionary<int, List<Transaction>>();

                await foreach(Transaction transaction in transactions.AsAsyncEnumerable())
                {
                    if (transactionsByProvider.TryGetValue(transaction.ProviderId, out var transactionList))
                    {
                        transactionList.Add(transaction);
                    }
                    else
                    {
                        transactionsByProvider[transaction.ProviderId] = new List<Transaction>(100) { transaction };
                    }
                }

                return Ok(transactionsByProvider.ToAsyncEnumerable());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetProviderReportAsync), null);
                return Problem();
            }
        }
    }
}
