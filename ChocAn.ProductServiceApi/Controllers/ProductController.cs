// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProductController.cs
// *
// * Description: Implements the Product controller for the ProductService API.
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

namespace ChocAn.ProductServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> logger;
        private readonly IRepository<Product> repository;
        private readonly PagingOptions defaultPagingOptions;
        public ProductController(
            ILogger<ProductController> logger,
            IOptions<PagingOptions> defaultPagingOptions,
            IRepository<Product> repository)
        {
            this.logger = logger;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
        }

        /// <summary>
        /// Retrieves all products from Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Product> sortOptions,
            [FromQuery] SearchOptions<Product> searchOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<Product> products = new();
                await foreach (Product product in repository.GetAllAsync(pagingOptions, sortOptions, searchOptions))
                {
                    products.Add(product);
                }
                return Ok(products);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }
        
        /// <summary>
        /// Retrieves an individual product from the Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <returns>200 on success. 404 if product does not exist. 500 on exception</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var product = await repository.GetAsync(id);
                if (null == product)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new product into the Product repository.
        /// </summary>
        /// <param name="product"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] Product product)
        {
            try
            {
                // Verify product's ID = 0 to enforce good behavior
                if (product.Id != 0)
                    return BadRequest();

                var result = await repository.AddAsync(new Product()
                {
                    Name = product.Name,
                    Cost = product.Cost,
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
        /// Updates a product in the Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <param name="product">Product updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Product product)
        {
            try
            {
                // Verify product's ID and the ID of the endpoint are the same
                if (product.Id != id)
                    return BadRequest();

                var numChanged = await repository.UpdateAsync(new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Cost = product.Cost
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
        /// Deletes a product from the Product respoitory.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <returns>200 on success. 404 if product does not exist. 500 on exception</returns>
        [HttpDelete("{id}", Name = nameof(DeleteAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var product = await repository.DeleteAsync(id);
                if (null == product)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
