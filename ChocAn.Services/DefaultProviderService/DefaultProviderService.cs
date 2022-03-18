// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProviderService.cs
// *
// * Description: Implememnts a class which uses an httpClientFactory object to 
// *   access the ChocAn ProviderSevice API
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
using System.Text.Json;
using ChocAn.ProviderRepository;
using Microsoft.Extensions.Logging;

namespace ChocAn.Services.DefaultProviderService
{
    public class DefaultProviderService : IProviderService
    {
        public const string ProviderErrorMessage = "Error while processing request for api/provider/{id}";
        public const string ProviderExceptionMessage = "Exception while processing request for api/provider/{id}";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<DefaultProviderService> logger;

        public static readonly string Name = ServiceNames.DefaultProviderService;

        /// <summary>
        /// Constructor for DefaultProviderService
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultProviderService(IHttpClientFactory httpClientFactory,
            ILogger<DefaultProviderService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves provider data from provider service
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///   A tuple consisting of the following fields:
        ///   isSuccess - A boolean specifying the success of the retrieve operation
        ///   provider - provider data
        ///   errorMessage - a string specifying the cause of the operation failure, null otherwise
        /// </returns>
        public async Task<(bool isSuccess, Provider? provider, string? errorMessage)> GetAsync(int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient(Name);
                var response = await client.GetAsync($"api/Provider/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var provider = JsonSerializer.Deserialize<Provider>(content, options);
                    return (true, provider, null);
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return (true, null, response.ReasonPhrase);
                }

                logger?.LogError(ProviderErrorMessage, response.ReasonPhrase);
                return (false, null, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ProviderExceptionMessage, id);
                return (false, null, ex.Message);
            }
        }
    }
}
