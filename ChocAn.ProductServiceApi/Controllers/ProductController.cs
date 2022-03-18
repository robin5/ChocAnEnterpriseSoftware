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

using ChocAn.Repository;
using Microsoft.AspNetCore.Mvc;
using ChocAn.ProductRepository;
using ChocAn.ProductServiceApi.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ChocAn.ProductServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> logger;
        private readonly IMapper mapper;
        private readonly IRepository<ProductRepository.Product> productRepository;
        public ProductController(
            ILogger<ProductController> logger,
            IMapper mapper,
            IRepository<ProductRepository.Product> productRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.productRepository = productRepository;
        }

        /// <summary>
        /// Retrieves all products from Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet(Name = nameof(GetAllAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<Product> products = new();
                await foreach (Product product in productRepository.GetAllAsync())
                {
                    products.Add(product);
                }

                return Ok(products);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }
        /// <summary>
        /// Retrieves an individual product from the Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <returns>200 on success. 404 if product does not exist. 500 on exception</returns>
        [HttpGet("{id}", Name = nameof(GetAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var product = await productRepository.GetAsync(id);
                if (null == product)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new product into the Product repository.
        /// </summary>
        /// <param name="productResource"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost(Name = nameof(PostAsync))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] ProductResource productResource)
        {
            try
            {
                var product = mapper.Map<Product>(productResource);
                await productRepository.AddAsync(product);
                return Created("", productResource);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }


        /// <summary>
        /// Updates a product in the Product repository.
        /// </summary>
        /// <param name="id">Product's identification number</param>
        /// <param name="productResource">Product updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}", Name = nameof(PutAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] ProductResource productResource)
        {
            try
            {
                var product = mapper.Map<Product>(productResource);
                product.Id = id;
                await productRepository.UpdateAsync(product);

                return Ok(productResource);
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
                var product = await productRepository.DeleteAsync(id);
                if (null == product)
                {
                    return NotFound();
                }
                return Ok(product);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
