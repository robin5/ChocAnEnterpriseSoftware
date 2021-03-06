// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProvidersController.cs
// *
// * Description: Implements the Provider controller for the ProviderService API.
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

namespace ChocAn.ProviderServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly ILogger<ProviderController> logger;
        private readonly IRepository<Provider> repository;
        private readonly PagingOptions defaultPagingOptions;
        public ProviderController(
            ILogger<ProviderController> logger,
            IRepository<Provider> repository,
            IOptions<PagingOptions> defaultPagingOptions)
        {
            this.logger = logger;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
        }

        /// <summary>
        /// Retrieves all providers from Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Provider> sortOptions,
            [FromQuery] SearchOptions<Provider> searchOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<Provider> providers = new();
                await foreach (Provider provider in repository.GetAllAsync(pagingOptions, sortOptions, searchOptions))
                {
                    providers.Add(provider);
                }
                return Ok(providers);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Retrieves an individual provider from the Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 404 if provider does not exist. 500 on exception</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var provider = await repository.GetAsync(id);
                if (null == provider)
                {
                    return NotFound();
                }
                return Ok(provider);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new provider into the Provider repository.
        /// </summary>
        /// <param name="provider"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] Provider provider)
        {
            try
            {
                // Verify provider's ID = 0 to enforce good behavior
                if (provider.Id != 0)
                    return BadRequest();

                var result = await repository.AddAsync(new Provider()
                {
                    Name = provider.Name,
                    Email = provider.Email,
                    StreetAddress = provider.StreetAddress,
                    City = provider.City,
                    State = provider.State,
                    ZipCode = provider.ZipCode
                });

                if (null == result)
                    return BadRequest();

                return Created("", result);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Updates a provider in the Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <param name="resource">Provider updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Provider provider)
        {
            try
            {
                // Verify provider's ID and the ID of the endpoint are the same
                if (provider.Id != id)
                    return BadRequest();

                var numChanged = await repository.UpdateAsync(new Provider
                {
                    Id = provider.Id,
                    Name = provider.Name,
                    Email = provider.Email,
                    StreetAddress = provider.StreetAddress,
                    City = provider.City,
                    State = provider.State,
                    ZipCode = provider.ZipCode
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
        /// Deletes a provider from the Provider respoitory.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 404 if provider does not exist. 500 on exception</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var provider = await repository.DeleteAsync(id);
                if (null == provider)
                {
                    return NotFound();
                }
                return Ok(provider);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
