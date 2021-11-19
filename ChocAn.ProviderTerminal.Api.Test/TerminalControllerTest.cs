// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: TerminalControllerTest.cs
// *
// * Description: Tests for TerminalController class
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
// **********************************************************************************using System;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using ChocAn.ProviderTerminal.Api.Controllers;
using ChocAn.ProviderTerminal.Api.Resources;
using ChocAn.ProviderRepository;
using ChocAn.MemberRepository;
using ChocAn.ProviderServiceRepository;
using ChocAn.TransactionRepository;
using ChocAn.MockRepositories;

namespace ChocAn.ProviderTerminal.Api.Test
{
    public class TerminalControllerTest
    {
        #region Useful Constants
        private readonly int PROVIDER_ID = 999999999;

        private readonly int MEMBER_ID = 999999998;
        private const string MEMBER_STATUS_ACTIVE = "active";

        private readonly int PROVIDER_SERVICE_ID = 999999;
        private const string PROVIDER_SERVICE_NAME = "Dietician";
        private const decimal PROVIDER_SERVICE_COST = 123.45M;

        private readonly DateTime TRANSACTION_SERVICE_DATE = DateTime.Now;
        private const string TRANSACTION_SERVICE_COMMENT = "1234567890";

        #endregion

        [Fact]
        public async Task ValidateTerminalMember_ExistingMember()
        {
            // Arrange
            var memberService = new MockMemberRepository();

            await memberService.AddAsync(new Member
            {
                Id = MEMBER_ID,
                Status = MEMBER_STATUS_ACTIVE
            });

            // Act
            var controller = new TerminalController(null, memberService, null, null, null);
            var result = await controller.Member(MEMBER_ID);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<MemberResource>(objectResult.Value);
            Assert.Equal(MEMBER_ID, resource.Id);
            Assert.Equal(MEMBER_STATUS_ACTIVE, resource.Status);
        }

        [Fact]
        public async Task ValidateTerminalMember_NonexistingMember()
        {
            // Arrange
            var memberService = new MockMemberRepository();

            // Act
            var controller = new TerminalController(null, memberService, null, null, null);
            var result = await controller.Member(MEMBER_ID);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateTerminalProvider_ExistingProvider()
        {
            // Arrange
            var providerService = new MockProviderRepository();

            await providerService.AddAsync(new Provider
            {
                Id = PROVIDER_ID
            });

            // Act
            var controller = new TerminalController(null, null, providerService, null, null);
            var result = await controller.Provider(PROVIDER_ID);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<ProviderResource>(objectResult.Value);
            Assert.Equal(PROVIDER_ID, resource.Id);
        }

        [Fact]
        public async Task ValidateTerminalProvider_NonexistingProvider()
        {
            // Arrange
            var providerService = new MockProviderRepository();

            // Act
            var controller = new TerminalController(null, null, providerService, null, null);
            var result = await controller.Provider(PROVIDER_ID);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateTerminalProviderService_ExistingProviderService()
        {
            // Arrange
            var providerServiceRepository = new MockProviderServiceRepository();

            await providerServiceRepository.AddAsync(new ProviderService
            {
                Id = PROVIDER_SERVICE_ID,
                Name = PROVIDER_SERVICE_NAME,
                Cost = PROVIDER_SERVICE_COST
            });

            // Act
            var controller = new TerminalController(null, null, null, providerServiceRepository, null);
            var result = await controller.ProviderService(PROVIDER_SERVICE_ID);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<ProviderServiceResource>(objectResult.Value);
            Assert.Equal(PROVIDER_SERVICE_ID, resource.Id);
            Assert.Equal(PROVIDER_SERVICE_NAME, resource.Name);
            Assert.Equal(PROVIDER_SERVICE_COST, resource.Cost);
        }

        [Fact]
        public async Task ValidateTerminalProviderService_NonexistingProviderService()
        {
            // Arrange
            var providerServiceService = new MockProviderServiceRepository();

            // Act
            var controller = new TerminalController(null, null, null, providerServiceService, null);
            var result = await controller.ProviderService(PROVIDER_SERVICE_ID);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateTransaction_ExistingProviderAndExistingMember()
        {
            // Arrange
            // Init mocked ProviderService
            var providerService = new MockProviderRepository();
            await providerService.AddAsync(new Provider
            {
                Id = PROVIDER_ID,
            });

            // Init mocked MemberService
            var memberService = new MockMemberRepository();
            await memberService.AddAsync(new Member
            {
                Id = MEMBER_ID,
                Status = MEMBER_STATUS_ACTIVE
            });

            // Init mocked ProviderServiceService
            var providerServiceService = new MockProviderServiceRepository();
            await providerServiceService.AddAsync(new ProviderService
            {
                Id = PROVIDER_SERVICE_ID,
                Name = PROVIDER_SERVICE_NAME,
                Cost = PROVIDER_SERVICE_COST
            });

            // Init mocked ProviderService
            var transactionService = new MockTransactionRepository();

            var terminalTransaction = new TransactionResource
            {
                MemberId = MEMBER_ID,
                ProviderId = PROVIDER_ID,
                ServiceId = PROVIDER_SERVICE_ID,
                ServiceDate = TRANSACTION_SERVICE_DATE,
                ServiceComment = TRANSACTION_SERVICE_COMMENT
            };

            // Act
            // Instantiate controller
            var controller = new TerminalController(null,
                memberService,
                providerService, 
                providerServiceService,
                transactionService);

            // Call controller
            var result = await controller.Transaction(terminalTransaction);

            // Assert
            var objectResult = Assert.IsType<CreatedResult>(result);
            var resource = objectResult.Value;

            var transactionResource = Assert.IsType<TransactionResource>(resource);

            // get all transaction from mock object
            List<Transaction> transactions = new List<Transaction>();
            await foreach (Transaction t in transactionService.GetAllAsync())
            {
                transactions.Add(t);
            }

            // Verify there is only one transaction
            Assert.Single(transactions);

            // Verify transaction was not altered by controller
            var transaction = transactions[0];

            Assert.Equal(MEMBER_ID, transaction.MemberId);
            Assert.Equal(PROVIDER_ID, transaction.ProviderId);
            Assert.Equal(PROVIDER_SERVICE_ID, transaction.ServiceId);
            Assert.Equal(TRANSACTION_SERVICE_DATE, transaction.ServiceDate);
            Assert.Equal(TRANSACTION_SERVICE_COMMENT, transaction.ServiceComment);
        }

        /*
            [Fact]
            public async Task ValidateTransaction_NonexistingProvider()
            {

            }

            [Fact]
            public async Task ValidateTransaction_NonExistingMember()
            {

            }
        */
    }
}
