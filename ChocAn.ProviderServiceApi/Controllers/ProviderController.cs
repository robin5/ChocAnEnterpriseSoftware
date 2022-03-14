﻿// **********************************************************************************
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

using ChocAn.Repository;
using Microsoft.AspNetCore.Mvc;
using ChocAn.ProviderRepository;
using ChocAn.ProviderServiceApi.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ChocAn.ProviderServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderController : ControllerBase
    {
        private readonly ILogger<ProviderController> logger;
        private readonly IMapper mapper;
        private readonly IRepository<ProviderRepository.Provider> providerRepository;
        public ProviderController(
            ILogger<ProviderController> logger,
            IMapper mapper,
            IRepository<ProviderRepository.Provider> providerRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.providerRepository = providerRepository;
        }

        /// <summary>
        /// Retrieves all providers from Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet(Name = nameof(GetAllAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<Provider> providers = new();
                await foreach (Provider provider in providerRepository.GetAllAsync())
                {
                    providers.Add(provider);
                }

                return Ok(providers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }
        /// <summary>
        /// Retrieves an individual provider from the Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 404 if provider does not exist. 500 on exception</returns>
        [HttpGet("{id}", Name = nameof(GetAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var provider = await providerRepository.GetAsync(id);
                if (null == provider)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<ProviderResource>(provider));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new provider into the Provider repository.
        /// </summary>
        /// <param name="providerResource"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost(Name = nameof(PostAsync))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] ProviderResource providerResource)
        {
            try
            {
                var provider = mapper.Map<Provider>(providerResource);
                await providerRepository.AddAsync(provider);
                return Created("", providerResource);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }


        /// <summary>
        /// Updates a provider in the Provider repository.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <param name="providerResource">Provider updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}", Name = nameof(PutAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] ProviderResource providerResource)
        {
            try
            {
                var provider = mapper.Map<Provider>(providerResource);
                provider.Id = id;
                await providerRepository.UpdateAsync(provider);

                return Ok(providerResource);
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
        /// Deletes a provider from the Provider respoitory.
        /// </summary>
        /// <param name="id">Provider's identification number</param>
        /// <returns>200 on success. 404 if provider does not exist. 500 on exception</returns>
        [HttpDelete("{id}", Name = nameof(DeleteAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var provider = await providerRepository.DeleteAsync(id);
                if (null == provider)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<ProviderResource>(provider));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
