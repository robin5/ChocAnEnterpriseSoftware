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

using ChocAn.ProviderRepository;
using ChocAn.ProviderServiceApi.Resources;

namespace ChocAn.Services.DefaultProviderService
{
    public class DefaultProviderService : DefaultService<ProviderResource, Provider>
    {
        public const string HttpClientName = Services.HttpClientName.ProviderService;
        public const string Url = ServiceUrl.ProviderService;

        /// <summary>
        /// Constructor for DefaultProviderService
        /// </summary>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultProviderService(IHttpClientFactory httpClientFactory)
            : base(Url, HttpClientName, httpClientFactory)
        {
        }
    }
}
