// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultTransactionService.cs
// *
// * Description: Implememnts a class which uses an httpClientFactory object to 
// *   access the ChocAn TransactionSevice API
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

using System.Text.Json;
using ChocAn.TransactionRepository;
using Microsoft.Extensions.Logging;

namespace ChocAn.Services.DefaultTransactionService
{
    public class DefaultTransactionService : ITransactionService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<DefaultTransactionService> logger;

        public static readonly string Name = ServiceNames.DefaultTransactionService;

        /// <summary>
        /// Constructor for DefaultTransactionService
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultTransactionService(IHttpClientFactory httpClientFactory,
            ILogger<DefaultTransactionService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves transaction data from transaction service
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///   A tuple consisting of the following fields:
        ///   isSuccess - A boolean specifying the success of the retrieve operation
        ///   transaction - transaction data
        ///   errorMessage - a string specifying the cause of the operation failure, null otherwise
        /// </returns>
        public async Task<(bool isSuccess, string? errorMessage)> AddAsync(Transaction transaction)
        {
            try
            {
                var client = httpClientFactory.CreateClient("DefaultTransactionService");
                var content = new StringContent(JsonSerializer.Serialize<Transaction>(transaction));
                var response = await client.PostAsync($"api/Transaction/", content);
                if (response.IsSuccessStatusCode)
                {
                    return (true, null);
                }
                return (false, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, ex.Message);
            }
        }
    }
}
