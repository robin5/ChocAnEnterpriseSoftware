// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: DefaultMemberService.cs
// *
// * Description: Implememnts a class which uses an httpClientFactory object to 
// *   access the ChocAn Sevice APIs
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
using System.Text;
using System.Net.Http.Headers;

namespace ChocAn.Services
{
    public abstract class DefaultService<TResource, TModel> : IService<TResource, TModel>
        where TResource : class
        where TModel : class
    {
        private readonly string url;
        private readonly IHttpClientFactory httpClientFactory;
        private readonly string httpClientName;

        private int offset = 0;
        private int limit = 25;
        private readonly List<string> search = new();
        private readonly List<string> orderby = new();

        public const string MemberErrorMessage = "Error while processing request for api/member/{id}";
        public const string MemberExceptionMessage = "Exception while processing request for api/member/{id}";
        public const string FindExceptionMessage = "Exception while processing request for api/member/{queryParams}";
        public const string FindErrorMessage = "Error while processing request for api/member?search=name eq {find}";

        /// <summary>
        /// Constructor for DefaultService<T>
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="url"></param>
        /// <param name="httpClientFactory"></param>
        /// <param name="logger"></param>
        public DefaultService(
            string url,
            string httpClientName,
            IHttpClientFactory httpClientFactory)
        {
            this.url = url;
            this.httpClientName = httpClientName;
            this.httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Adds pagination options to the query
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public IService<TResource, TModel> Paginate(int offset, int limit)
        {
            this.offset = offset;
            this.limit = limit;
            return this;
        }

        /// <summary>
        /// Adds search options to the query
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IService<TResource, TModel> AddSearch(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                search.Add(value);
            }
            return this;
        }

        /// <summary>
        /// Adds Orderby options to the query
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IService<TResource, TModel> OrderBy(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                orderby.Add(value);
            }
            return this;
        }

        /// <summary>
        /// Assembles query parameters from pagination, search, and 
        /// orderBy fields
        /// </summary>
        /// <returns></returns>
        private string QueryParams()
        {
            StringBuilder sb = new();
            sb.Append($"?offset={offset}");
            sb.Append($"&limit={limit}");
            search.ForEach(search => { sb.Append("&search="); sb.Append(search); });
            orderby.ForEach(orderby => { sb.Append("&orderby="); sb.Append(orderby); });
            return sb.ToString();
        }

        /// <summary>
        /// Retrieves T data from service
        /// </summary>
        /// <param name="id"></param>
        /// <returns>
        ///   A tuple consisting of the following fields:
        ///   isSuccess - A boolean specifying the success of the retrieve operation
        ///   member - member data
        ///   errorMessage - a string specifying the cause of the operation failure, null otherwise
        /// </returns>
        public async virtual Task<(bool isSuccess, TModel? result, string? errorMessage)> GetAsync(int id)
        {
            using var client = httpClientFactory.CreateClient(httpClientName);
            var response = await client.GetAsync($"{url}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<TModel>(content, options);
                return (true, result, null);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return (true, null, response.ReasonPhrase);
            }

            return (false, null, response.ReasonPhrase);
        }

        /// <summary>
        /// Retrieves 0 or more T entities from service.  Response is modified by
        /// pagination, sort, and search parameters
        /// </summary>
        /// <returns></returns>
        public async virtual Task<(bool isSuccess, IEnumerable<TModel>? result, string? errorMessage)> GetAllAsync()
        {
            using var client = httpClientFactory.CreateClient(httpClientName);

            var response = await client.GetAsync($"{url}{QueryParams()}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<List<TModel>>(content, options);
                return (true, result, null);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return (true, null, response.ReasonPhrase);
            }

            return (false, null, response.ReasonPhrase);
        }

        /// <summary>
        /// Creates a T entity.
        /// </summary>
        /// <param name="entity">Entity to create</param>
        /// <returns>TResource representing created entity</returns>
        public async virtual Task<(bool isSuccess, TModel? result, string? errorMessage)> CreateAsync(TResource entity)
        {
            using var client = httpClientFactory.CreateClient(httpClientName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize<TResource>(entity);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{url}", content);
            if (response.IsSuccessStatusCode)
            {
                var bytes = await response.Content.ReadAsByteArrayAsync();
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<TModel>(bytes, options);
                return (true, result, null);
            }

            return (false, null, response.ReasonPhrase);
        }

        /// <summary>
        /// Updates a T entity.
        /// </summary>
        /// <param name="id">ID of entity to update</param>
        /// <param name="entity">New entity values</param>
        /// <returns>Tuple representing result</returns>
        public async Task<(bool isSuccess, string? errorMessage)> UpdateAsync(int id, TResource entity)
        {
            using var client = httpClientFactory.CreateClient(httpClientName);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var json = JsonSerializer.Serialize<TResource>(entity);
            var content = new StringContent(json.ToString(), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{url}/{id}", content);
            if (response.IsSuccessStatusCode)
            {
                return (true, null);
            }

            return (false, response.ReasonPhrase);
        }

        /// <summary>
        /// Deletes a T entity
        /// </summary>
        /// <param name="id">ID of entity to delete</param>
        /// <returns>Deleted T entity</returns>
        public async Task<(bool isSuccess, TModel? result, string? errorMessage)> DeleteAsync(int id)
        {
            using var client = httpClientFactory.CreateClient(httpClientName);
            var response = await client.DeleteAsync($"{url}/{id}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                var options = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
                var result = JsonSerializer.Deserialize<TModel>(content, options);
                return (true, result, null);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return (true, null, response.ReasonPhrase);
            }

            return (false, null, response.ReasonPhrase);
        }
    }
}
