// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProviderRepositoryTest.cs
// *
// * Description: Defines tests for DefaultProviderRepository class
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
using Microsoft.EntityFrameworkCore;
using Xunit;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Search;
using ChocAn.Repository.Sorting;
using ChocAn.Data;

namespace ChocAn.ProviderRepository.Test
{
    /// <summary>
    /// Tests for DefaultProviderRepository class
    /// </summary>
    public class DefaultProviderRepositoryTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const int NON_EXISTENT_PROVIDER_ID = 52;

        private const int VALID0_ID = 999999999;
        private const string VALID0_NAME = "1234567890123456789012345";
        private const string VALID0_EMAIL = "tester0@chocan.com";
        private const string VALID0_ADDRESS = "1234567890123456789012345";
        private const string VALID0_CITY = "12345678901234";
        private const string VALID0_STATE = "12";
        private const int VALID0_ZIPCODE = 99999;
        private const string VALID0_STATUS = "Status 0";

        private const int VALID1_ID = 20;
        private const string VALID1_NAME = "Name 1";
        private const string VALID1_EMAIL = "tester1@chocan.com";
        private const string VALID1_ADDRESS = "Address 1";
        private const string VALID1_CITY = "City 1";
        private const string VALID1_STATE = "WA";
        private const int VALID1_ZIPCODE = 20001;
        private const string VALID1_STATUS = "Status 1";

        private const int VALID2_ID = 30;
        private const string VALID2_NAME = "Name 2";
        private const string VALID2_EMAIL = "tester2@chocan.com";
        private const string VALID2_ADDRESS = "Address 2";
        private const string VALID2_CITY = "City 2";
        private const string VALID2_STATE = "OR";
        private const int VALID2_ZIPCODE = 30002;
        private const string VALID2_STATUS = "Status 2";

        private const string VALID_UPDATE_NAME = "1234567890";
        private const string VALID_UPDATE_EMAIL = "update@chocan.com";
        private const string VALID_UPDATE_ADDRESS = "1234567890123";
        private const string VALID_UPDATE_CITY = "1232345";
        private const string VALID_UPDATE_STATE = "CA";
        private const int VALID_UPDATE_ZIPCODE = 10026;
        private const string VALID_UPDATE_STATUS = "suspended";
        #endregion

        /// <summary>
        /// Creates an instance of an InMemory database
        /// </summary>
        /// <param name="name">Name of InMemory database instance</param>
        /// <returns></returns>
        private static ProviderDbContext GetContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<ProviderDbContext>()
                .UseInMemoryDatabase(name).Options;

            return new ProviderDbContext(dbOptions);
        }

        /// <summary>
        /// Inserts 1 valid provider into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task InsertValidProviderIntoTestDatabase(string name)
        {
            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext(name);
            // Arrange
            var provider = new Provider
            {
                Id = VALID0_ID,
                Name = VALID0_NAME,
                Email = VALID0_EMAIL,
                StreetAddress = VALID0_ADDRESS,
                City = VALID0_CITY,
                State = VALID0_STATE,
                ZipCode = VALID0_ZIPCODE
            };

            context.Add<Provider>(provider);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Inserts 3 valid providers into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidProvidersIntoTestDatabase(string name)
        {
            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext(name);
            // Arrange
            context.Add<Provider>(new Provider
            {
                Id = VALID0_ID,
                Name = VALID0_NAME,
                Email = VALID0_EMAIL,
                StreetAddress = VALID0_ADDRESS,
                City = VALID0_CITY,
                State = VALID0_STATE,
                ZipCode = VALID0_ZIPCODE
            });
            context.Add<Provider>(new Provider
            {
                Id = VALID1_ID,
                Name = VALID1_NAME,
                Email = VALID1_EMAIL,
                StreetAddress = VALID1_ADDRESS,
                City = VALID1_CITY,
                State = VALID1_STATE,
                ZipCode = VALID1_ZIPCODE
            });
            context.Add<Provider>(new Provider
            {
                Id = VALID2_ID,
                Name = VALID2_NAME,
                Email = VALID2_EMAIL,
                StreetAddress = VALID2_ADDRESS,
                City = VALID2_CITY,
                State = VALID2_STATE,
                ZipCode = VALID2_ZIPCODE
            });

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifies Inserting a provider into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            var provider = new Provider
            {
                Id = VALID0_ID,
                Name = VALID0_NAME,
                Email = VALID0_EMAIL,
                StreetAddress = VALID0_ADDRESS,
                City = VALID0_CITY,
                State = VALID0_STATE,
                ZipCode = VALID0_ZIPCODE
            };

            // Act
            using (ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("Add"))
            {
                var repository = new DefaultProviderRepository(context);
                var result = await repository.AddAsync(provider);
            }

            // Assert
            using (ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("Add"))
            {
                var result = context.Find<Provider>(VALID0_ID);

                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_EMAIL, result.Email);
                Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
                Assert.Equal(VALID0_CITY, result.City);
                Assert.Equal(VALID0_STATE, result.State);
                Assert.Equal(VALID0_ZIPCODE, result.ZipCode);
            }
        }

        /// <summary>
        /// Verifies getting a provider from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProviderAsync()
        {
            // Arrange
            await DefaultProviderRepositoryTest.InsertValidProviderIntoTestDatabase("Get");

            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("Get");
            // Act
            var repository = new DefaultProviderRepository(context);
            var result = await repository.GetAsync(VALID0_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VALID0_ID, result.Id);
            Assert.Equal(VALID0_NAME, result.Name);
            Assert.Equal(VALID0_EMAIL, result.Email);
            Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
            Assert.Equal(VALID0_CITY, result.City);
            Assert.Equal(VALID0_STATE, result.State);
            Assert.Equal(VALID0_ZIPCODE, result.ZipCode);
        }

        /// <summary>
        /// Verifies getting a nonexistent provider from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetProviderAsyncNonExistentProvider()
        {
            // Arrange
            await DefaultProviderRepositoryTest.InsertValidProviderIntoTestDatabase("ValidateGetProviderAsyncNonExistentProvider");

            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("ValidateGetProviderAsyncNonExistentProvider");
            // Act
            var repository = new DefaultProviderRepository(context);
            var result = await repository.GetAsync(NON_EXISTENT_PROVIDER_ID);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Verifies updating a provider in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultProviderRepositoryTest.InsertValidProviderIntoTestDatabase("Update");

            var providerChanges = new Provider
            {
                Id = VALID0_ID,
                Name = VALID_UPDATE_NAME,
                Email = VALID_UPDATE_EMAIL,
                StreetAddress = VALID_UPDATE_ADDRESS,
                City = VALID_UPDATE_CITY,
                State = VALID_UPDATE_STATE,
                ZipCode = VALID_UPDATE_ZIPCODE
            };

            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("Update");
            // Act
            var repository = new DefaultProviderRepository(context);
            var result = await repository.UpdateAsync(providerChanges);

            // Assert
            // Validate return value of function call
            Assert.Equal(1, result);

            // Validate provider was updated in the database
            var provider = await context.Providers.FindAsync(VALID0_ID);
            Assert.NotNull(provider);
            Assert.Equal(VALID0_ID, provider.Id);
            Assert.Equal(VALID_UPDATE_NAME, provider.Name);
            Assert.Equal(VALID_UPDATE_EMAIL, provider.Email);
            Assert.Equal(VALID_UPDATE_ADDRESS, provider.StreetAddress);
            Assert.Equal(VALID_UPDATE_CITY, provider.City);
            Assert.Equal(VALID_UPDATE_STATE, provider.State);
            Assert.Equal(VALID_UPDATE_ZIPCODE, provider.ZipCode);
        }

        /// <summary>
        /// Verifies deleting a provider from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultProviderRepositoryTest.InsertValidProviderIntoTestDatabase("Delete");

            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("Delete");
            // Act
            var repository = new DefaultProviderRepository(context);
            var result = await repository.DeleteAsync(VALID0_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(VALID0_ID, result.Id);
            Assert.Equal(VALID0_NAME, result.Name);
            Assert.Equal(VALID0_EMAIL, result.Email);
            Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
            Assert.Equal(VALID0_CITY, result.City);
            Assert.Equal(VALID0_STATE, result.State);
            Assert.Equal(VALID0_ZIPCODE, result.ZipCode);

            Assert.Equal(0, await context.Providers.CountAsync());
        }

        /// <summary>
        /// Verifies getting all providers from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllProvidersAsync()
        {
            // Arrange
            await DefaultProviderRepositoryTest.Insert3ValidProvidersIntoTestDatabase("GetAllProvidersAsync");

            bool provider0Found = false;
            bool provider1Found = false;
            bool provider2Found = false;

            using ProviderDbContext context = DefaultProviderRepositoryTest.GetContext("GetAllProvidersAsync");
            // Act
            var repository = new DefaultProviderRepository(context);

            // Assert
            await foreach (Provider provider in repository.GetAllAsync(
                new PagingOptions() { Offset = 0, Limit = 3 },
                new SortOptions<Provider>(),
                new SearchOptions<Provider>()))
            {
                if (VALID0_ID == provider.Id)
                {
                    Assert.Equal(VALID0_NAME, provider.Name);
                    Assert.Equal(VALID0_EMAIL, provider.Email);
                    Assert.Equal(VALID0_ADDRESS, provider.StreetAddress);
                    Assert.Equal(VALID0_CITY, provider.City);
                    Assert.Equal(VALID0_STATE, provider.State);
                    Assert.Equal(VALID0_ZIPCODE, provider.ZipCode);
                    provider0Found = true;
                }
                else if (VALID1_ID == provider.Id)
                {
                    Assert.Equal(VALID1_NAME, provider.Name);
                    Assert.Equal(VALID1_EMAIL, provider.Email);
                    Assert.Equal(VALID1_ADDRESS, provider.StreetAddress);
                    Assert.Equal(VALID1_CITY, provider.City);
                    Assert.Equal(VALID1_STATE, provider.State);
                    Assert.Equal(VALID1_ZIPCODE, provider.ZipCode);
                    provider1Found = true;
                }
                else if (VALID2_ID == provider.Id)
                {
                    Assert.Equal(VALID2_NAME, provider.Name);
                    Assert.Equal(VALID2_EMAIL, provider.Email);
                    Assert.Equal(VALID2_ADDRESS, provider.StreetAddress);
                    Assert.Equal(VALID2_CITY, provider.City);
                    Assert.Equal(VALID2_STATE, provider.State);
                    Assert.Equal(VALID2_ZIPCODE, provider.ZipCode);
                    provider2Found = true;
                }
            }

            // There should be 3 providers in the database
            Assert.True(provider0Found);
            Assert.True(provider1Found);
            Assert.True(provider2Found);
        }
    }
}
