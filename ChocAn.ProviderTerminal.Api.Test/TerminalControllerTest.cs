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
using ChocAn.ProviderService;
using ChocAn.MemberService;
using ChocAn.ProviderServiceService;
using ChocAn.TransactionService;
using ChocAn.MockRepositories;

namespace ChocAn.ProviderTerminal.Api.Test
{
    public class TerminalControllerTest
    {
        #region Useful Constants
        private readonly Guid PROVIDER_ID = Guid.NewGuid();
        private const decimal PROVIDER_NUMBER = 999999999;

        private readonly Guid MEMBER_ID = Guid.NewGuid();
        private const decimal MEMBER_NUMBER = 999999999;
        private const string MEMBER_STATUS_ACTIVE = "active";

        private readonly Guid PROVIDER_SERVICE_ID = Guid.NewGuid();
        private const decimal PROVIDER_SERVICE_CODE = 999999;
        private const string PROVIDER_SERVICE_NAME = "Dietician";
        private const decimal PROVIDER_SERVICE_COST = 123.45M;

        private readonly DateTime TRANSACTION_SERVICE_DATE = DateTime.Now;
        private const string TRANSACTION_SERVICE_COMMENT = "1234567890";

        #endregion

        #region MOQ version
        /*
        [Fact]
        public async Task ValidateTerminalProvider_ExistingProvider_MOQVersion()
        {
            // Arrange
            const decimal providerNumber = 42;

            var providerService = new Mock<IProviderService>();
            providerService
                .Setup(x => x.GetProviderByNumberAsync(providerNumber))
                .Returns(Task.FromResult(new Provider { Number = providerNumber }));

            // Act
            var controller = new TerminalController(null, providerService.Object, null, null, null);
            var result = await controller.TerminalProvider(providerNumber);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<ProviderResource>(objectResult.Value);
            Assert.Equal(providerNumber, resource.Number);
        }
        */
        #endregion

        [Fact]
        public async Task ValidateTerminalProvider_ExistingProvider()
        {
            // Arrange
            var providerService = new ChocAn.MockRepositories.MockProviderRepository();

            await providerService.AddAsync(new Provider
            {
                Number = PROVIDER_NUMBER
            });

            // Act
            var controller = new TerminalController(null, providerService, null, null, null);
            var result = await controller.TerminalProvider(PROVIDER_NUMBER);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<ProviderResource>(objectResult.Value);
            Assert.Equal(PROVIDER_NUMBER, resource.Number);
        }

        [Fact]
        public async Task ValidateTerminalProvider_NonexistingProvider()
        {
            // Arrange
            var providerService = new MockProviderRepository();

            // Act
            var controller = new TerminalController(null, providerService, null, null, null);
            var result = await controller.TerminalProvider(PROVIDER_NUMBER);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateTerminalMember_ExistingMember()
        {
            // Arrange
            var memberService = new MockMemberRepository();

            await memberService.AddAsync(new Member
            {
                Number = MEMBER_NUMBER,
                Status = MEMBER_STATUS_ACTIVE
            });

            // Act
            var controller = new TerminalController(null, null, memberService, null, null);
            var result = await controller.TerminalMember(MEMBER_NUMBER);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<MemberResource>(objectResult.Value);
            Assert.Equal(MEMBER_NUMBER, resource.Number);
            Assert.Equal(MEMBER_STATUS_ACTIVE, resource.Status);
        }

        [Fact]
        public async Task ValidateTerminalMember_NonexistingMember()
        {
            // Arrange
            var memberService = new MockMemberRepository();

            // Act
            var controller = new TerminalController(null, null, memberService, null, null);
            var result = await controller.TerminalMember(MEMBER_NUMBER);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task ValidateTerminalProviderService_ExistingProviderService()
        {
            // Arrange
            var providerServiceService = new MockProviderServiceRepository();

            await providerServiceService.AddAsync(new ProviderServiceService.ProviderService
            {
                Id = PROVIDER_ID,
                Code = PROVIDER_SERVICE_CODE,
                Name = PROVIDER_SERVICE_NAME,
                Cost = PROVIDER_SERVICE_COST
            });

            // Act
            var controller = new TerminalController(null, null, null, providerServiceService, null);
            var result = await controller.TerminalProviderService(PROVIDER_SERVICE_CODE);

            // Assert

            var objectResult = Assert.IsType<OkObjectResult>(result);
            var resource = Assert.IsType<ProviderServiceResource>(objectResult.Value);
            Assert.Equal(PROVIDER_SERVICE_CODE, resource.Code);
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
            var result = await controller.TerminalProviderService(PROVIDER_SERVICE_CODE);

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
                Number = PROVIDER_NUMBER
            });

            // Init mocked MemberService
            var memberService = new MockMemberRepository();
            await memberService.AddAsync(new Member
            {
                Id = MEMBER_ID,
                Number = MEMBER_NUMBER,
                Status = MEMBER_STATUS_ACTIVE
            });

            // Init mocked ProviderServiceService
            var providerServiceService = new MockProviderServiceRepository();
            await providerServiceService.AddAsync(new ProviderServiceService.ProviderService
            {
                Id = PROVIDER_SERVICE_ID,
                Code = PROVIDER_SERVICE_CODE,
                Name = PROVIDER_SERVICE_NAME,
                Cost = PROVIDER_SERVICE_COST
            });

            // Init mocked ProviderService
            var transactionService = new MockTransactionRepository();

            var terminalTransaction = new TransactionResource
            {
                ProviderNumber = PROVIDER_NUMBER,
                MemberNumber = MEMBER_NUMBER,
                ServiceCode = PROVIDER_SERVICE_CODE,
                ServiceDate = TRANSACTION_SERVICE_DATE,
                ServiceComment = TRANSACTION_SERVICE_COMMENT
            };

            // Act
            // Instantiate controller
            var controller = new TerminalController(null, 
                providerService, 
                memberService, 
                providerServiceService,
                transactionService);

            // Call controller
            var result = await controller.Transaction(terminalTransaction);

            // Assert
            var objectResult = Assert.IsType<CreatedResult>(result);
            var resource = objectResult.Value;

            var transactionResource = Assert.IsType<TransactionResource>(resource);

            Assert.Equal("accepted", transactionResource.Status);

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
            Assert.Equal(PROVIDER_SERVICE_CODE, transaction.ServiceCode);
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