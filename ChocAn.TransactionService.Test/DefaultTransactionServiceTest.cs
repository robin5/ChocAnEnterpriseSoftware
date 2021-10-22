// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultTransactionServiceTest.cs
// *
// * Description: Tests for DefaultTransactionService class
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

namespace ChocAn.TransactionService.Test
{
    /// <summary>
    /// Tests for DefaultTransactionService class
    /// </summary>
    public class DefaultTransactionServiceTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const string NON_EXISTENT_MEMBER_ID = "5bd1ffc8-9047-4219-b9a2-ae2e25db8118";

        private const string VALID0_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca57e";
        private const string VALID0_PROVIDER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca57f";
        private const string VALID0_MEMBER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca57a";
        private static readonly DateTime VALID0_SERVICE_DATETIME = new DateTime(2021, 1, 1);
        private const decimal VALID0_SERVICE_CODE = 999999;
        private const string VALID0_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime VALID0_TRANSACTION_DATETIME = new DateTime(2021, 1, 2);

        private const string VALID1_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca58e";
        private const string VALID1_PROVIDER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca58f";
        private const string VALID1_MEMBER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca58a";
        private static readonly DateTime VALID1_SERVICE_DATETIME = new DateTime(2021, 1, 3);
        private const decimal VALID1_SERVICE_CODE = 1;
        private const string VALID1_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime VALID1_TRANSACTION_DATETIME = new DateTime(2021, 1, 4);

        private const string VALID2_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca59e";
        private const string VALID2_PROVIDER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca59f";
        private const string VALID2_MEMBER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca59a";
        private static readonly DateTime VALID2_SERVICE_DATETIME = new DateTime(2021, 1, 5);
        private const decimal VALID2_SERVICE_CODE = 2;
        private const string VALID2_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime VALID2_TRANSACTION_DATETIME = new DateTime(2021, 1, 6);

        private const string VALID_UPDATE_PROVIDER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca60f";
        private const string VALID_UPDATE_MEMBER_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca60a";
        private static readonly DateTime VALID_UPDATE_SERVICE_DATETIME = new DateTime(2021, 1, 7);
        private const decimal VALID_UPDATE_SERVICE_CODE = 3;
        private const string VALID_UPDATE_SERVICE_COMMENT = "1234567890123456789012345";
        private static readonly DateTime VALID_UPDATE_TRANSACTION_DATETIME = new DateTime(2021, 1, 8);
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
            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext(name))
            {
                // Arrange
                var transaction = new Transaction
                {
                    Id = new Guid(VALID0_ID),
                    ProviderId = new Guid(VALID0_PROVIDER_ID),
                    MemberId = new Guid(VALID0_MEMBER_ID),
                    ServiceDateTime = VALID0_SERVICE_DATETIME,
                    ServiceCode = VALID0_SERVICE_CODE,
                    ServiceComment = VALID0_SERVICE_COMMENT,
                    TransactionDateTime = VALID0_TRANSACTION_DATETIME
                };

                context.Add<Transaction>(transaction);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Inserts 3 valid transactions into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidTransactionsIntoTestDatabase(string name)
        {
            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext(name))
            {
                // Arrange
                context.Add<Transaction>(new Transaction
                {
                    Id = new Guid(VALID0_ID),
                    ProviderId = new Guid(VALID0_PROVIDER_ID),
                    MemberId = new Guid(VALID0_MEMBER_ID),
                    ServiceDateTime = VALID0_SERVICE_DATETIME,
                    ServiceCode = VALID0_SERVICE_CODE,
                    ServiceComment = VALID0_SERVICE_COMMENT,
                    TransactionDateTime = VALID0_TRANSACTION_DATETIME
                });
                context.Add<Transaction>(new Transaction
                {
                    Id = new Guid(VALID1_ID),
                    ProviderId = new Guid(VALID1_PROVIDER_ID),
                    MemberId = new Guid(VALID1_MEMBER_ID),
                    ServiceDateTime = VALID1_SERVICE_DATETIME,
                    ServiceCode = VALID1_SERVICE_CODE,
                    ServiceComment = VALID1_SERVICE_COMMENT,
                    TransactionDateTime = VALID1_TRANSACTION_DATETIME
                });
                context.Add<Transaction>(new Transaction
                {
                    Id = new Guid(VALID2_ID),
                    ProviderId = new Guid(VALID2_PROVIDER_ID),
                    MemberId = new Guid(VALID2_MEMBER_ID),
                    ServiceDateTime = VALID2_SERVICE_DATETIME,
                    ServiceCode = VALID2_SERVICE_CODE,
                    ServiceComment = VALID2_SERVICE_COMMENT,
                    TransactionDateTime = VALID2_TRANSACTION_DATETIME
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifies Inserting a transaction into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            Guid validId = new Guid(VALID0_ID);

            var transaction = new Transaction
            {
                Id = validId,
                ProviderId = new Guid(VALID0_PROVIDER_ID),
                MemberId = new Guid(VALID0_MEMBER_ID),
                ServiceDateTime = VALID0_SERVICE_DATETIME,
                ServiceCode = VALID0_SERVICE_CODE,
                ServiceComment = VALID0_SERVICE_COMMENT,
                TransactionDateTime = VALID0_TRANSACTION_DATETIME
            };

            // Act
            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("Add"))
            {
                var defaultTransactionService = new DefaultTransactionService(context);
                var result = await defaultTransactionService.AddAsync(transaction);
            }

            // Assert
            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("Add"))
            {
                var result = context.Find<Transaction>(validId);

                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_PROVIDER_ID, result.ProviderId.ToString());
                Assert.Equal(VALID0_MEMBER_ID, result.MemberId.ToString());
                Assert.Equal(VALID0_SERVICE_DATETIME, result.ServiceDateTime);
                Assert.Equal(VALID0_SERVICE_CODE, result.ServiceCode);
                Assert.Equal(VALID0_SERVICE_COMMENT, result.ServiceComment);
                Assert.Equal(VALID0_TRANSACTION_DATETIME, result.TransactionDateTime);
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
            await DefaultTransactionServiceTest.InsertValidTransactionIntoTestDatabase("Get");
            var validId = new Guid(VALID0_ID);

            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("Get"))
            {
                // Act
                var defaultTransactionService = new DefaultTransactionService(context);
                var result = await defaultTransactionService.GetAsync(validId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_PROVIDER_ID, result.ProviderId.ToString());
                Assert.Equal(VALID0_MEMBER_ID, result.MemberId.ToString());
                Assert.Equal(VALID0_SERVICE_DATETIME, result.ServiceDateTime);
                Assert.Equal(VALID0_SERVICE_CODE, result.ServiceCode);
                Assert.Equal(VALID0_SERVICE_COMMENT, result.ServiceComment);
                Assert.Equal(VALID0_TRANSACTION_DATETIME, result.TransactionDateTime);
            }
        }

        /// <summary>
        /// Verifies getting a nonexistent transaction from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetTransactionAsyncNonExistentTransaction()
        {
            // Arrange
            await DefaultTransactionServiceTest.InsertValidTransactionIntoTestDatabase("ValidateGetTransactionAsyncNonExistentTransaction");
            var nonExistentTransactionId = new Guid(NON_EXISTENT_MEMBER_ID);

            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("ValidateGetTransactionAsyncNonExistentTransaction"))
            {
                // Act
                var defaultTransactionService = new DefaultTransactionService(context);
                var result = await defaultTransactionService.GetAsync(nonExistentTransactionId);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Verifies updating a transaction in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultTransactionServiceTest.InsertValidTransactionIntoTestDatabase("Update");

            var validId = new Guid(VALID0_ID);

            var transactionChanges = new Transaction
            {
                Id = validId,
                ProviderId = new Guid(VALID_UPDATE_PROVIDER_ID),
                MemberId = new Guid(VALID_UPDATE_MEMBER_ID),
                ServiceDateTime = VALID_UPDATE_SERVICE_DATETIME,
                ServiceCode = VALID_UPDATE_SERVICE_CODE,
                ServiceComment = VALID_UPDATE_SERVICE_COMMENT,
                TransactionDateTime = VALID_UPDATE_TRANSACTION_DATETIME
            };

            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("Update"))
            {
                // Act
                var defaultTransactionService = new DefaultTransactionService(context);
                var result = await defaultTransactionService.UpdateAsync(transactionChanges);

                // Assert
                // Validate return value of function call
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID_UPDATE_PROVIDER_ID, result.ProviderId.ToString());
                Assert.Equal(VALID_UPDATE_MEMBER_ID, result.MemberId.ToString());
                Assert.Equal(VALID_UPDATE_SERVICE_DATETIME, result.ServiceDateTime);
                Assert.Equal(VALID_UPDATE_SERVICE_CODE, result.ServiceCode);
                Assert.Equal(VALID_UPDATE_SERVICE_COMMENT, result.ServiceComment);
                Assert.Equal(VALID_UPDATE_TRANSACTION_DATETIME, result.TransactionDateTime);

                // Validate transaction was updated in the database
                var transaction = await context.Transactions.FindAsync(validId);
                Assert.NotNull(transaction);
                Assert.Equal(validId, transaction.Id);
                Assert.Equal(VALID_UPDATE_PROVIDER_ID, transaction.ProviderId.ToString());
                Assert.Equal(VALID_UPDATE_MEMBER_ID, transaction.MemberId.ToString());
                Assert.Equal(VALID_UPDATE_SERVICE_DATETIME, transaction.ServiceDateTime);
                Assert.Equal(VALID_UPDATE_SERVICE_CODE, transaction.ServiceCode);
                Assert.Equal(VALID_UPDATE_SERVICE_COMMENT, transaction.ServiceComment);
                Assert.Equal(VALID_UPDATE_TRANSACTION_DATETIME, transaction.TransactionDateTime);
            }
        }

        /// <summary>
        /// Verifies deleting a transaction from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultTransactionServiceTest.InsertValidTransactionIntoTestDatabase("Delete");

            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("Delete"))
            {
                // Act
                var defaultTransactionService = new DefaultTransactionService(context);
                var result = await defaultTransactionService.DeleteAsync(new Guid(VALID0_ID));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id.ToString());
                Assert.Equal(VALID0_PROVIDER_ID, result.ProviderId.ToString());
                Assert.Equal(VALID0_MEMBER_ID, result.MemberId.ToString());
                Assert.Equal(VALID0_SERVICE_DATETIME, result.ServiceDateTime);
                Assert.Equal(VALID0_SERVICE_CODE, result.ServiceCode);
                Assert.Equal(VALID0_SERVICE_COMMENT, result.ServiceComment);
                Assert.Equal(VALID0_TRANSACTION_DATETIME, result.TransactionDateTime);

                Assert.Equal(0, await context.Transactions.CountAsync());
            }
        }

        /// <summary>
        /// Verifies getting all transactions from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllTransactionsAsync()
        {
            // Arrange
            await DefaultTransactionServiceTest.Insert3ValidTransactionsIntoTestDatabase("GetAllTransactionsAsync");

            bool transaction0Found = false;
            bool transaction1Found = false;
            bool transaction2Found = false;

            using (TransactionDbContext context = DefaultTransactionServiceTest.GetContext("GetAllTransactionsAsync"))
            {
                // Act
                var defaultTransactionService = new DefaultTransactionService(context);

                // Assert
                await foreach (Transaction transaction in defaultTransactionService.GetAllAsync())
                {
                    if (VALID0_ID == transaction.Id.ToString())
                    {
                        transaction0Found = true;
                        Assert.Equal(VALID0_PROVIDER_ID, transaction.ProviderId.ToString());
                        Assert.Equal(VALID0_MEMBER_ID, transaction.MemberId.ToString());
                        Assert.Equal(VALID0_SERVICE_DATETIME, transaction.ServiceDateTime);
                        Assert.Equal(VALID0_SERVICE_CODE, transaction.ServiceCode);
                        Assert.Equal(VALID0_SERVICE_COMMENT, transaction.ServiceComment);
                        Assert.Equal(VALID0_TRANSACTION_DATETIME, transaction.TransactionDateTime);
                    }
                    else if (VALID1_ID == transaction.Id.ToString())
                    {
                        transaction1Found = true;
                        Assert.Equal(VALID1_PROVIDER_ID, transaction.ProviderId.ToString());
                        Assert.Equal(VALID1_MEMBER_ID, transaction.MemberId.ToString());
                        Assert.Equal(VALID1_SERVICE_DATETIME, transaction.ServiceDateTime);
                        Assert.Equal(VALID1_SERVICE_CODE, transaction.ServiceCode);
                        Assert.Equal(VALID1_SERVICE_COMMENT, transaction.ServiceComment);
                        Assert.Equal(VALID1_TRANSACTION_DATETIME, transaction.TransactionDateTime);
                    }
                    else if (VALID2_ID == transaction.Id.ToString())
                    {
                        transaction2Found = true;
                        Assert.Equal(VALID2_PROVIDER_ID, transaction.ProviderId.ToString());
                        Assert.Equal(VALID2_MEMBER_ID, transaction.MemberId.ToString());
                        Assert.Equal(VALID2_SERVICE_DATETIME, transaction.ServiceDateTime);
                        Assert.Equal(VALID2_SERVICE_CODE, transaction.ServiceCode);
                        Assert.Equal(VALID2_SERVICE_COMMENT, transaction.ServiceComment);
                        Assert.Equal(VALID2_TRANSACTION_DATETIME, transaction.TransactionDateTime);
                    }
                }

                // There should be 3 transactions in the database
                Assert.True(transaction0Found);
                Assert.True(transaction1Found);
                Assert.True(transaction2Found);
            }
        }
    }
}
