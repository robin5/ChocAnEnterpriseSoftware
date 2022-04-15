// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultTransactionRepositoryTest.cs
// *
// * Description: Tests for DefaultTransactionRepository class
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

using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.TransactionRepository.Test
{
    /// <summary>
    /// Tests for DefaultTransactionRepository class
    /// </summary>
    public class DefaultTransactionRepositoryTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const int NON_EXISTENT_MEMBER_ID = 50;

        private const int T0_ID = 10;
        private const int T0_PROVIDER_ID = 20;
        private const int T0_MEMBER_ID = 30;
        private static readonly DateTime T0_SERVICE_DATETIME = new(2021, 1, 1);
        private const int T0_PRODUCT_ID = 999999;
        private const string T0_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime T0_CREATED = new(2021, 1, 2);

        private const int T1_ID = 11;
        private const int T1_PROVIDER_ID = 21;
        private const int T1_MEMBER_ID = 31;
        private static readonly DateTime T1_SERVICE_DATETIME = new(2021, 1, 3);
        private const int T1_PRODUCT_ID = 1;
        private const string T1_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime T1_CREATED = new(2021, 1, 4);

        private const int T2_ID = 12;
        private const int T2_PROVIDER_ID = 22;
        private const int T2_MEMBER_ID = 32;
        private static readonly DateTime T2_SERVICE_DATETIME = new(2021, 1, 5);
        private const int T2_PRODUCT_ID = 2;
        private const string T2_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime T2_CREATED = new(2021, 1, 6);

        private const int T_UPDATE_PROVIDER_ID = 23;
        private const int T_UPDATE_MEMBER_ID = 33;
        private static readonly DateTime T_UPDATE_SERVICE_DATETIME = new(2021, 1, 7);
        private const int T_UPDATE_PRODUCT_ID = 3;
        private const string T_UPDATE_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime T_UPDATE_CREATED = new(2021, 1, 8);
        #endregion

        /// <summary>
        /// Creates an instance of an InMemory database
        /// </summary>
        /// <param name="name">Name of InMemory database instance</param>
        /// <returns></returns>
        private static TransactionDbContext GetContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<TransactionDbContext>()
                .UseInMemoryDatabase(name).Options;

            return new TransactionDbContext(dbOptions);
        }

        /// <summary>
        /// Inserts 1 valid transaction into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task InsertValidTransactionIntoTestDatabase(string name)
        {
            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext(name);
            // Arrange
            var transaction = new Transaction
            {
                Id = T0_ID,
                ProviderId = T0_PROVIDER_ID,
                MemberId = T0_MEMBER_ID,
                ProductId = T0_PRODUCT_ID,
                ServiceDate = T0_SERVICE_DATETIME,
                ServiceComment = T0_SERVICE_COMMENT,
                Created = T0_CREATED
            };

            context.Add<Transaction>(transaction);
            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Inserts 3 valid transactions into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidTransactionsIntoTestDatabase(string name)
        {
            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext(name);
            // Arrange
            context.Add<Transaction>(new Transaction
            {
                Id = T0_ID,
                ProviderId = T0_PROVIDER_ID,
                MemberId = T0_MEMBER_ID,
                ProductId = T0_PRODUCT_ID,
                ServiceDate = T0_SERVICE_DATETIME,
                ServiceComment = T0_SERVICE_COMMENT,
                Created = T0_CREATED
            });
            context.Add<Transaction>(new Transaction
            {
                Id = T1_ID,
                ProviderId = T1_PROVIDER_ID,
                MemberId = T1_MEMBER_ID,
                ProductId = T1_PRODUCT_ID,
                ServiceDate = T1_SERVICE_DATETIME,
                ServiceComment = T1_SERVICE_COMMENT,
                Created = T1_CREATED
            });
            context.Add<Transaction>(new Transaction
            {
                Id = T2_ID,
                ProviderId = T2_PROVIDER_ID,
                MemberId = T2_MEMBER_ID,
                ProductId = T2_PRODUCT_ID,
                ServiceDate = T2_SERVICE_DATETIME,
                ServiceComment = T2_SERVICE_COMMENT,
                Created = T2_CREATED
            });

            await context.SaveChangesAsync();
        }

        /// <summary>
        /// Verifies Inserting a transaction into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            DateTime beforeTransactionTime = DateTime.Now;

            var transaction = new Transaction
            {
                Id = T0_ID,
                ProviderId = T0_PROVIDER_ID,
                MemberId = T0_MEMBER_ID,
                ProductId = T0_PRODUCT_ID,
                ServiceDate = T0_SERVICE_DATETIME,
                ServiceComment = T0_SERVICE_COMMENT,
                // Note: Created is set by database
            };

            // Act
            using (TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("Add"))
            {
                var defaultTransactionService = new DefaultTransactionRepository(context);
                var result = await defaultTransactionService.AddAsync(transaction);
            }

            // Assert
            using (TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("Add"))
            {
                var result = context.Find<Transaction>(T0_ID);

                Assert.NotNull(result);
                Assert.Equal(T0_ID, result.Id);
                Assert.Equal(T0_PROVIDER_ID, result.ProviderId);
                Assert.Equal(T0_MEMBER_ID, result.MemberId);
                Assert.Equal(T0_PRODUCT_ID, result.ProductId);
                Assert.Equal(T0_SERVICE_DATETIME, result.ServiceDate);
                Assert.Equal(T0_SERVICE_COMMENT, result.ServiceComment);
                Assert.True(beforeTransactionTime <= result.Created);
            }
        }

        /// <summary>
        /// Verifies getting a transaction from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetTransactionAsync()
        {
            // Arrange
            await DefaultTransactionRepositoryTest.InsertValidTransactionIntoTestDatabase("Get");

            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("Get");
            // Act
            var defaultTransactionService = new DefaultTransactionRepository(context);
            var result = await defaultTransactionService.GetAsync(T0_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(T0_ID, result.Id);
            Assert.Equal(T0_PROVIDER_ID, result.ProviderId);
            Assert.Equal(T0_MEMBER_ID, result.MemberId);
            Assert.Equal(T0_PRODUCT_ID, result.ProductId);
            Assert.Equal(T0_SERVICE_DATETIME, result.ServiceDate);
            Assert.Equal(T0_SERVICE_COMMENT, result.ServiceComment);
            Assert.Equal(T0_CREATED, result.Created);
        }

        /// <summary>
        /// Verifies getting a nonexistent transaction from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetTransactionAsyncNonExistentTransaction()
        {
            // Arrange
            await DefaultTransactionRepositoryTest.InsertValidTransactionIntoTestDatabase("ValidateGetTransactionAsyncNonExistentTransaction");
            var nonExistentTransactionId = NON_EXISTENT_MEMBER_ID;

            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("ValidateGetTransactionAsyncNonExistentTransaction");
            // Act
            var defaultTransactionService = new DefaultTransactionRepository(context);
            var result = await defaultTransactionService.GetAsync(nonExistentTransactionId);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Verifies updating a transaction in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultTransactionRepositoryTest.InsertValidTransactionIntoTestDatabase("Update");

            var transactionChanges = new Transaction
            {
                Id = T0_ID,
                ProviderId = T_UPDATE_PROVIDER_ID,
                MemberId = T_UPDATE_MEMBER_ID,
                ProductId = T_UPDATE_PRODUCT_ID,
                ServiceDate = T_UPDATE_SERVICE_DATETIME,
                ServiceComment = T_UPDATE_SERVICE_COMMENT,
                Created = T_UPDATE_CREATED
            };

            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("Update");
            // Act
            var defaultTransactionService = new DefaultTransactionRepository(context);
            var result = await defaultTransactionService.UpdateAsync(transactionChanges);

            // Assert
            // Validate return value of function call
            Assert.NotNull(result);
            Assert.Equal(T0_ID, result.Id);
            Assert.Equal(T_UPDATE_PROVIDER_ID, result.ProviderId);
            Assert.Equal(T_UPDATE_MEMBER_ID, result.MemberId);
            Assert.Equal(T_UPDATE_PRODUCT_ID, result.ProductId);
            Assert.Equal(T_UPDATE_SERVICE_DATETIME, result.ServiceDate);
            Assert.Equal(T_UPDATE_SERVICE_COMMENT, result.ServiceComment);
            Assert.Equal(T_UPDATE_CREATED, result.Created);

            // Validate transaction was updated in the database
            var transaction = await context.Transactions.FindAsync(T0_ID);
            Assert.NotNull(transaction);
            Assert.Equal(T0_ID, transaction.Id);
            Assert.Equal(T_UPDATE_PROVIDER_ID, transaction.ProviderId);
            Assert.Equal(T_UPDATE_MEMBER_ID, transaction.MemberId);
            Assert.Equal(T_UPDATE_PRODUCT_ID, transaction.ProductId);
            Assert.Equal(T_UPDATE_SERVICE_DATETIME, transaction.ServiceDate);
            Assert.Equal(T_UPDATE_SERVICE_COMMENT, transaction.ServiceComment);
            Assert.Equal(T_UPDATE_CREATED, transaction.Created);
        }

        /// <summary>
        /// Verifies deleting a transaction from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultTransactionRepositoryTest.InsertValidTransactionIntoTestDatabase("Delete");

            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("Delete");
            // Act
            var defaultTransactionService = new DefaultTransactionRepository(context);
            var result = await defaultTransactionService.DeleteAsync(T0_ID);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(T0_ID, result.Id);
            Assert.Equal(T0_PROVIDER_ID, result.ProviderId);
            Assert.Equal(T0_MEMBER_ID, result.MemberId);
            Assert.Equal(T0_PRODUCT_ID, result.ProductId);
            Assert.Equal(T0_SERVICE_DATETIME, result.ServiceDate);
            Assert.Equal(T0_SERVICE_COMMENT, result.ServiceComment);
            Assert.Equal(T0_CREATED, result.Created);

            Assert.Equal(0, await context.Transactions.CountAsync());
        }

        /// <summary>
        /// Verifies getting all transactions from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllTransactionsAsync()
        {
            // Arrange
            await DefaultTransactionRepositoryTest.Insert3ValidTransactionsIntoTestDatabase("GetAllTransactionsAsync");

            bool transaction0Found = false;
            bool transaction1Found = false;
            bool transaction2Found = false;

            using TransactionDbContext context = DefaultTransactionRepositoryTest.GetContext("GetAllTransactionsAsync");
            // Act
            var defaultTransactionService = new DefaultTransactionRepository(context);

            // Assert
            await foreach (Transaction transaction in defaultTransactionService.GetAllAsync(
                    new PagingOptions() { Offset = 0, Limit = 3 }, 
                    new SortOptions<Transaction>(),
                    new SearchOptions<Transaction>()))
            {
                if (T0_ID == transaction.Id)
                {
                    transaction0Found = true;
                    Assert.Equal(T0_PROVIDER_ID, transaction.ProviderId);
                    Assert.Equal(T0_MEMBER_ID, transaction.MemberId);
                    Assert.Equal(T0_PRODUCT_ID, transaction.ProductId);
                    Assert.Equal(T0_SERVICE_DATETIME, transaction.ServiceDate);
                    Assert.Equal(T0_SERVICE_COMMENT, transaction.ServiceComment);
                    Assert.Equal(T0_CREATED, transaction.Created);
                }
                else if (T1_ID == transaction.Id)
                {
                    transaction1Found = true;
                    Assert.Equal(T1_PROVIDER_ID, transaction.ProviderId);
                    Assert.Equal(T1_MEMBER_ID, transaction.MemberId);
                    Assert.Equal(T1_PRODUCT_ID, transaction.ProductId);
                    Assert.Equal(T1_SERVICE_DATETIME, transaction.ServiceDate);
                    Assert.Equal(T1_SERVICE_COMMENT, transaction.ServiceComment);
                    Assert.Equal(T1_CREATED, transaction.Created);
                }
                else if (T2_ID == transaction.Id)
                {
                    transaction2Found = true;
                    Assert.Equal(T2_PROVIDER_ID, transaction.ProviderId);
                    Assert.Equal(T2_MEMBER_ID, transaction.MemberId);
                    Assert.Equal(T2_PRODUCT_ID, transaction.ProductId);
                    Assert.Equal(T2_SERVICE_DATETIME, transaction.ServiceDate);
                    Assert.Equal(T2_SERVICE_COMMENT, transaction.ServiceComment);
                    Assert.Equal(T2_CREATED, transaction.Created);
                }
            }

            // There should be 3 transactions in the database
            Assert.True(transaction0Found);
            Assert.True(transaction1Found);
            Assert.True(transaction2Found);
        }
    }
}
