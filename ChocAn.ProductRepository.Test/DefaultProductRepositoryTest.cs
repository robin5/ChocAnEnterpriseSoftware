// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProductRepositoryTest.cs
// *
// * Description: Tests for DefaultProductRepository class
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

using System.Threading.Tasks;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ChocAn.ProductRepository.Test
{
    /// <summary>
    /// Tests for DefaultProductRepository class
    /// </summary>
    public class DefaultProductRepositoryTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const int NON_EXISTENT_MEMBER_ID = 50;

        private const int VALID0_ID = 999999;
        private const string VALID0_NAME = "1234567890123456789012345";
        private const decimal VALID0_COST = 213.99M;

        private const int VALID1_ID = 20;
        private const string VALID1_NAME = "Name 1";
        private const decimal VALID1_COST = 18.18M;

        private const int VALID2_ID = 30;
        private const string VALID2_NAME = "Name 2";
        private const decimal VALID2_COST = 107.00M;

        private const string VALID_UPDATE_NAME = "1234567890";
        private const decimal VALID_UPDATE_COST = 74.34M;
        #endregion

        /// <summary>
        /// Creates an instance of an InMemory database
        /// </summary>
        /// <param name="name">Name of InMemory database instance</param>
        /// <returns></returns>
        private static ProductDbContext GetContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(name).Options;

            return new ProductDbContext(dbOptions);
        }

        /// <summary>
        /// Inserts 1 valid product into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task InsertValidProductIntoTestDatabase(string name)
        {
            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext(name))
            {
                // Arrange
                var product = new Product
                {
                    Id = VALID0_ID,
                    Name = VALID0_NAME,
                    Cost = VALID0_COST
                };

                context.Add<Product>(product);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Inserts 3 valid products into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidProductsIntoTestDatabase(string name)
        {
            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext(name))
            {
                // Arrange
                context.Add<Product>(new Product
                {
                    Id = VALID0_ID,
                    Name = VALID0_NAME,
                    Cost = VALID0_COST
                });
                context.Add<Product>(new Product
                {
                    Id = VALID1_ID,
                    Name = VALID1_NAME,
                    Cost = VALID1_COST
                });
                context.Add<Product>(new Product
                {
                    Id = VALID2_ID,
                    Name = VALID2_NAME,
                    Cost = VALID2_COST
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifies Inserting a product into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            var product = new Product
            {
                Id = VALID0_ID,
                Name = VALID0_NAME,
                Cost = VALID0_COST
            };

            // Act
            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("Add"))
            {
                var repository = new DefaultProductRepository(context);
                var result = await repository.AddAsync(product);
            }

            // Assert
            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("Add"))
            {
                var result = context.Find<Product>(VALID0_ID);

                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);
            }
        }

        /// <summary>
        /// Verifies getting a product from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProductAsync()
        {
            // Arrange
            await DefaultProductRepositoryTest.InsertValidProductIntoTestDatabase("Get");

            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("Get"))
            {
                // Act
                var repository = new DefaultProductRepository(context);
                var result = await repository.GetAsync(VALID0_ID);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);
            }
        }

        /// <summary>
        /// Verifies getting a nonexistent product from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProductAsyncNonExistentProduct()
        {
            // Arrange
            await DefaultProductRepositoryTest.InsertValidProductIntoTestDatabase("ValidateGetProductAsyncNonExistentProduct");

            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("ValidateGetProductAsyncNonExistentProduct"))
            {
                // Act
                var repository = new DefaultProductRepository(context);
                var result = await repository.GetAsync(NON_EXISTENT_MEMBER_ID);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Verifies updating a product in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultProductRepositoryTest.InsertValidProductIntoTestDatabase("Update");

            var productChanges = new Product
            {
                Id = VALID0_ID,
                Name = VALID_UPDATE_NAME,
                Cost = VALID_UPDATE_COST
            };

            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("Update"))
            {
                // Act
                var repository = new DefaultProductRepository(context);
                var result = await repository.UpdateAsync(productChanges);

                // Assert
                // Validate return value of function call
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id);
                Assert.Equal(VALID_UPDATE_NAME, result.Name);
                Assert.Equal(VALID_UPDATE_COST, result.Cost);

                // Validate product was updated in the database
                var product = await context.Products.FindAsync(VALID0_ID);
                Assert.NotNull(product);
                Assert.Equal(VALID0_ID, product.Id);
                Assert.Equal(VALID_UPDATE_NAME, product.Name);
                Assert.Equal(VALID_UPDATE_COST, product.Cost);
            }
        }

        /// <summary>
        /// Verifies deleting a product from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultProductRepositoryTest.InsertValidProductIntoTestDatabase("Delete");

            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("Delete"))
            {
                // Act
                var repository = new DefaultProductRepository(context);
                var result = await repository.DeleteAsync(VALID0_ID);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);

                Assert.Equal(0, await context.Products.CountAsync());
            }
        }

        /// <summary>
        /// Verifies getting all products from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllProductsAsync()
        {
            // Arrange
            await DefaultProductRepositoryTest.Insert3ValidProductsIntoTestDatabase("GetAllProductsAsync");

            bool product0Found = false;
            bool product1Found = false;
            bool product2Found = false;

            using (ProductDbContext context = DefaultProductRepositoryTest.GetContext("GetAllProductsAsync"))
            {
                // Act
                var repository = new DefaultProductRepository(context);

                // Assert
                await foreach (Product product in repository.GetAllAsync(
                    new PagingOptions() { Offset = 0, Limit = 3 }, 
                    new SortOptions<Product>()))
                {
                    if (VALID0_ID == product.Id)
                    {
                        Assert.Equal(VALID0_NAME, product.Name);
                        Assert.Equal(VALID0_COST, product.Cost);
                        product0Found = true;
                    }
                    else if (VALID1_ID == product.Id)
                    {
                        Assert.Equal(VALID1_NAME, product.Name);
                        Assert.Equal(VALID1_COST, product.Cost);
                        product1Found = true;
                    }
                    else if (VALID2_ID == product.Id)
                    {
                        Assert.Equal(VALID2_NAME, product.Name);
                        Assert.Equal(VALID2_COST, product.Cost);
                        product2Found = true;
                    }
                }

                // There should be 3 products in the database
                Assert.True(product0Found);
                Assert.True(product1Found);
                Assert.True(product2Found);
            }
        }
    }
}
