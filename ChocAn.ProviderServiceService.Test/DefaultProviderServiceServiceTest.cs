// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProviderServiceServiceTest.cs
// *
// * Description: Tests for DefaultProviderServiceService class
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

using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChocAn.ProviderServiceService.Test
{
    /// <summary>
    /// Tests for DefaultProviderServiceService class
    /// </summary>
    public class DefaultProviderServiceServiceTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const string NON_EXISTENT_MEMBER_ID = "5bd1ffc8-9047-4219-b9a2-ae2e25db8118";

        private const string VALID0_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca57e";
        private const decimal VALID0_CODE = 999999;
        private const string VALID0_NAME = "1234567890123456789012345";
        private const decimal VALID0_COST = 213.99M;

        private const string VALID1_ID = "3b3170f8-963d-4729-a687-ae2e25db7bb7";
        private const decimal VALID1_CODE = 1;
        private const string VALID1_NAME = "Name 1";
        private const decimal VALID1_COST = 18.18M;

        private const string VALID2_ID = "dec047b3-1918-40bd-b9a2-f137a44429a5";
        private const decimal VALID2_CODE = 2;
        private const string VALID2_NAME = "Name 2";
        private const decimal VALID2_COST = 107.00M;

        private const decimal VALID_UPDATE_CODE = 123456;
        private const string VALID_UPDATE_NAME = "1234567890";
        private const decimal VALID_UPDATE_COST = 74.34M;
        #endregion

        /// <summary>
        /// Creates an instance of an InMemory database
        /// </summary>
        /// <param name="name">Name of InMemory database instance</param>
        /// <returns></returns>
        private static ProviderServiceDbContext GetContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<ProviderServiceDbContext>()
                .UseInMemoryDatabase(name).Options;

            return new ProviderServiceDbContext(dbOptions);
        }

        /// <summary>
        /// Inserts 1 valid providerService into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task InsertValidProviderServiceIntoTestDatabase(string name)
        {
            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext(name))
            {
                // Arrange
                var providerService = new ProviderService
                {
                    Id = new Guid(VALID0_ID),
                    Code = VALID0_CODE,
                    Name = VALID0_NAME,
                    Cost = VALID0_COST
                };

                context.Add<ProviderService>(providerService);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Inserts 3 valid providerServices into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidProviderServicesIntoTestDatabase(string name)
        {
            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext(name))
            {
                // Arrange
                context.Add<ProviderService>(new ProviderService
                {
                    Id = new Guid(VALID0_ID),
                    Code = VALID0_CODE,
                    Name = VALID0_NAME,
                    Cost = VALID0_COST
                });
                context.Add<ProviderService>(new ProviderService
                {
                    Id = new Guid(VALID1_ID),
                    Code = VALID1_CODE,
                    Name = VALID1_NAME,
                    Cost = VALID1_COST
                });
                context.Add<ProviderService>(new ProviderService
                {
                    Id = new Guid(VALID2_ID),
                    Code = VALID2_CODE,
                    Name = VALID2_NAME,
                    Cost = VALID2_COST
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifies Inserting a providerService into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            Guid validId = new Guid(VALID0_ID);

            var providerService = new ProviderService
            {
                Id = validId,
                Code = VALID0_CODE,
                Name = VALID0_NAME,
                Cost = VALID0_COST
            };

            // Act
            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("Add"))
            {
                var defaultProviderServiceService = new DefaultProviderServiceService(context);
                var result = await defaultProviderServiceService.AddAsync(providerService);
            }

            // Assert
            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("Add"))
            {
                var result = context.Find<ProviderService>(validId);

                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_CODE, result.Code);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);
            }
        }

        /// <summary>
        /// Verifies getting a providerService from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProviderServiceAsync()
        {
            // Arrange
            await DefaultProviderServiceServiceTest.InsertValidProviderServiceIntoTestDatabase("Get");
            var validId = new Guid(VALID0_ID);

            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("Get"))
            {
                // Act
                var defaultProviderServiceService = new DefaultProviderServiceService(context);
                var result = await defaultProviderServiceService.GetAsync(validId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_CODE, result.Code);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);
            }
        }

        /// <summary>
        /// Verifies getting a nonexistent providerService from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProviderServiceAsyncNonExistentProviderService()
        {
            // Arrange
            await DefaultProviderServiceServiceTest.InsertValidProviderServiceIntoTestDatabase("ValidateGetProviderServiceAsyncNonExistentProviderService");
            var nonExistentProviderServiceId = new Guid(NON_EXISTENT_MEMBER_ID);

            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("ValidateGetProviderServiceAsyncNonExistentProviderService"))
            {
                // Act
                var defaultProviderServiceService = new DefaultProviderServiceService(context);
                var result = await defaultProviderServiceService.GetAsync(nonExistentProviderServiceId);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Verifies updating a providerService in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultProviderServiceServiceTest.InsertValidProviderServiceIntoTestDatabase("Update");

            var validId = new Guid(VALID0_ID);

            var providerServiceChanges = new ProviderService
            {
                Id = validId,
                Code = VALID_UPDATE_CODE,
                Name = VALID_UPDATE_NAME,
                Cost = VALID_UPDATE_COST
            };

            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("Update"))
            {
                // Act
                var defaultProviderServiceService = new DefaultProviderServiceService(context);
                var result = await defaultProviderServiceService.UpdateAsync(providerServiceChanges);

                // Assert
                // Validate return value of function call
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID_UPDATE_CODE, result.Code);
                Assert.Equal(VALID_UPDATE_NAME, result.Name);
                Assert.Equal(VALID_UPDATE_COST, result.Cost);

                // Validate providerService was updated in the database
                var providerService = await context.ProviderServices.FindAsync(validId);
                Assert.NotNull(providerService);
                Assert.Equal(validId, providerService.Id);
                Assert.Equal(VALID_UPDATE_CODE, providerService.Code);
                Assert.Equal(VALID_UPDATE_NAME, providerService.Name);
                Assert.Equal(VALID_UPDATE_COST, providerService.Cost);
            }
        }

        /// <summary>
        /// Verifies deleting a providerService from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultProviderServiceServiceTest.InsertValidProviderServiceIntoTestDatabase("Delete");

            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("Delete"))
            {
                // Act
                var defaultProviderServiceService = new DefaultProviderServiceService(context);
                var result = await defaultProviderServiceService.DeleteAsync(new Guid(VALID0_ID));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id.ToString());
                Assert.Equal(VALID0_CODE, result.Code);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_COST, result.Cost);

                Assert.Equal(0, await context.ProviderServices.CountAsync());
            }
        }

        /// <summary>
        /// Verifies getting all providerServices from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllProviderServicesAsync()
        {
            // Arrange
            await DefaultProviderServiceServiceTest.Insert3ValidProviderServicesIntoTestDatabase("GetAllProviderServicesAsync");

            bool providerService0Found = false;
            bool providerService1Found = false;
            bool providerService2Found = false;

            using (ProviderServiceDbContext context = DefaultProviderServiceServiceTest.GetContext("GetAllProviderServicesAsync"))
            {
                // Act
                var defaultProviderServiceService = new DefaultProviderServiceService(context);

                // Assert
                await foreach (ProviderService providerService in defaultProviderServiceService.GetAllAsync())
                {
                    if (VALID0_ID == providerService.Id.ToString())
                    {
                        Assert.Equal(VALID0_CODE, providerService.Code);
                        Assert.Equal(VALID0_NAME, providerService.Name);
                        Assert.Equal(VALID0_COST, providerService.Cost);
                        providerService0Found = true;
                    }
                    else if (VALID1_ID == providerService.Id.ToString())
                    {
                        Assert.Equal(VALID1_CODE, providerService.Code);
                        Assert.Equal(VALID1_NAME, providerService.Name);
                        Assert.Equal(VALID1_COST, providerService.Cost);
                        providerService1Found = true;
                    }
                    else if (VALID2_ID == providerService.Id.ToString())
                    {
                        Assert.Equal(VALID2_CODE, providerService.Code);
                        Assert.Equal(VALID2_NAME, providerService.Name);
                        Assert.Equal(VALID2_COST, providerService.Cost);
                        providerService2Found = true;
                    }
                }

                // There should be 3 providerServices in the database
                Assert.True(providerService0Found);
                Assert.True(providerService1Found);
                Assert.True(providerService2Found);
            }
        }
    }
}
