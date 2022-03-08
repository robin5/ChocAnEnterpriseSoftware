// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultMemberService.cs
// *
// * Description: Implememnts a class which uses an httpClientFactory object to 
// *   access the ChocAn MemberSevice API
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
using ChocAn.MemberRepository;
using Microsoft.Extensions.Logging;

namespace ChocAn.Services.DefaultMemberService
{
    public class DefaultMemberService : IMemberService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<DefaultMemberService> logger;

        public static readonly string Name = ServiceNames.DefaultMemberService;

        /// <summary>
        /// Constructor for DefaultMemberService
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultMemberService(IHttpClientFactory httpClientFactory,
            ILogger<DefaultMemberService> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        /// <summary>
        /// Retrieves member data from member service
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///   A tuple consisting of the following fields:
        ///   isSuccess - A boolean specifying the success of the retrieve operation
        ///   member - member data
        ///   errorMessage - a string specifying the cause of the operation failure, null otherwise
        /// </returns>
        public async Task<(bool isSuccess, Member? member, string? errorMessage)> GetAsync(int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient(Name);
                var response = await client.GetAsync($"api/Member/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsByteArrayAsync();
                    var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                    var member = JsonSerializer.Deserialize<Member>(content, options);
                    return (true, member, null);
                }
                return (false, null, response.ReasonPhrase);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
