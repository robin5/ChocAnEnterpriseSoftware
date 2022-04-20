// **********************************************************************************
// * Copyright (c) 2022 Robin Murray
// **********************************************************************************
// *
// * File: MemberControllerTest.cs
// *
// * Description: Defines tests for DefaultMemberRepository class
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
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ChocAn.Repository;
using ChocAn.MemberRepository;
using ChocAn.MemberServiceApi.Controllers;
using ChocAn.MemberServiceApi.Resources;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository.Paging;
using Microsoft.Extensions.Options;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.MemberServiceApi.Test
{
    public class DefaultMemberControllerTest
    {
        /// <summary>
        /// Returns a  tuple consisting of a mock MemberResource and a mock Member
        /// </summary>
        /// <param name="id">The value to set the Id property of the instantiated Member object</param>
        /// <returns></returns>
        private static (MemberResource, Member) CreateResourceAndMember(int id = 1)
        {
            var resource = new MemberResource()
            {
                Name = $"Member{id}",
                Email = $"member{id}@chocan.com",
                StreetAddress = $"{id} Main Street",
                City = "Any City USA",
                State = "WA",
                ZipCode = 10000 + id,
                Status = "active"
            };

            var member = new Member()
            {
                Id = id,
                Name = new string(resource.Name),
                Email = new string(resource.Email),
                StreetAddress = new string(resource.StreetAddress),
                City = new string(resource.City),
                State = new string(resource.State),
                ZipCode = resource.ZipCode,
                Status = new string(resource.Status)
            };

            return (resource, member);
        }

        /// <summary>
        /// Returns a member object created from the fields of the MemberResource parameter
        /// The ID property of the returned Member object is set to 0
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private static Member MemberFromResource(MemberResource resource)
        {
            return new Member()
            {
                Id = 0,
                Name = resource.Name,
                Email = resource.Email,
                StreetAddress = resource.StreetAddress,
                City = resource.City,
                State = resource.State,
                ZipCode = resource.ZipCode,
                Status = resource.Status
            };
        }

        /// <summary>
        /// A class that implements an Asynchronous generator for mocking 
        /// the memberRepository.GetAllAsync() method
        /// </summary>
        private class MemberGenerator
        {
            private readonly IEnumerable<Member> members;

            /// <summary>
            /// Constructor initializes members to be returnd by Members property
            /// </summary>
            /// <param name="members"></param>
            public MemberGenerator(IEnumerable<Member> members)
            {
                this.members = members;
            }

            /// <summary>
            /// Asynchronous return of each member from the members list
            /// </summary>
            /// <returns>Member from members list</returns>
            public async IAsyncEnumerable<Member> Members()
            {
                foreach (Member member in members)
                {
                    await Task.Delay(0);
                    yield return member;
                }
            }
        }

        /// <summary>
        /// Verifies MemberController.GetAllAsync()
        /// 1) returns an OkObjectResult
        /// 2) the OkObjectResult contains a list of the 3 members from the repository
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllAsync()
        {
            // Arrange
            var (_, member1) = CreateResourceAndMember(1);
            var (_, member2) = CreateResourceAndMember(2);
            var (_, member3) = CreateResourceAndMember(3);

            var generator = new MemberGenerator(
                new List<Member> { member1, member2, member3 });

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.GetAllAsync(It.IsAny<PagingOptions>(), It.IsAny<SortOptions<Member>>(), It.IsAny<SearchOptions<Member>>()))
                .Returns(generator.Members);

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // Act
            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.GetAllAsync(
                new PagingOptions(), 
                new SortOptions<Member>(), 
                new SearchOptions<Member>());

            // Assert
            Assert.NotNull(result);

            // Verify call returns OkObjectResult
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            // Verify list contains all three members
            var list = Assert.IsType<List<Member>>(okObjectResult.Value);
            Assert.Equal<int>(3, list.Count);

            // Verify member 1
            var m1 = list.Find(m => m.Id == member1.Id);
            Assert.NotNull(m1);
            Assert.Equal(member1.Id, m1!.Id);
            Assert.Equal(member1.Name, m1!.Name);
            Assert.Equal(member1.Email, m1!.Email);
            Assert.Equal(member1.StreetAddress, m1!.StreetAddress);
            Assert.Equal(member1.City, m1!.City);
            Assert.Equal(member1.State, m1!.State);
            Assert.Equal(member1.ZipCode, m1!.ZipCode);
            Assert.Equal(member1.Status, m1!.Status);

            // Verify member 2
            var m2 = list.Find(m => m.Id == member2.Id);
            Assert.NotNull(m2);
            Assert.Equal(member2.Id, m2!.Id);
            Assert.Equal(member2.Name, m2!.Name);
            Assert.Equal(member2.Email, m2!.Email);
            Assert.Equal(member2.StreetAddress, m2!.StreetAddress);
            Assert.Equal(member2.City, m2!.City);
            Assert.Equal(member2.State, m2!.State);
            Assert.Equal(member2.ZipCode, m2!.ZipCode);
            Assert.Equal(member2.Status, m2!.Status);

            // Verify member 3
            var m3 = list.Find(m => m.Id == member3.Id);
            Assert.NotNull(m3);
            Assert.Equal(member3.Id, m3!.Id);
            Assert.Equal(member3.Name, m3!.Name);
            Assert.Equal(member3.Email, m3!.Email);
            Assert.Equal(member3.StreetAddress, m3!.StreetAddress);
            Assert.Equal(member3.City, m3!.City);
            Assert.Equal(member3.State, m3!.State);
            Assert.Equal(member3.ZipCode, m3!.ZipCode);
            Assert.Equal(member3.Status, m3!.Status);

        }

        /// <summary>
        /// Verifies MemberController.GetAllAsync() handles an Exception thrown by repository by:
        /// 1) logging the exception
        /// 2) returning an ObjectResult with StatusCode value of 500
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllAsyncException()
        {
            // * Arrange *

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.GetAllAsync(
                    It.IsAny<PagingOptions>(), 
                    It.IsAny<SortOptions<Member>>(),
                    It.IsAny<SearchOptions<Member>>()))
                .Throws<Exception>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.GetAllAsync(
                new PagingOptions() { Offset = 0, Limit = 3 }, 
                new SortOptions<Member>(),
                new SearchOptions<Member>());

            // * Assert *

            // Verify repository's GetAllAsync method was called
            mockRepository.Verify(repository => repository.GetAllAsync(
                It.IsAny<PagingOptions>(), 
                It.IsAny<SortOptions<Member>>(), 
                It.IsAny<SearchOptions<Member>>()), Times.Once());

            // Verify controller returned an ObjectResult whose status code is 500
            Assert.NotNull(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            // Verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.GetAllAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies MemberController.GetAsync(id) returns an OkObjectResult if the member is found
        /// Verifies the value of the OkObjectResult is the found member entity
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAsync()
        {
            // * Arrange *

            var (_, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.GetAsync(member.Id))
                .Returns(Task.FromResult(member));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.GetAsync(member.Id);

            // * Assert *

            // Verify call returns OkObjectResult
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            // Verify the member returned by OkObjectResult
            var value = Assert.IsType<Member>(okObjectResult.Value);
            Assert.Equal(member.Id, value.Id);
            Assert.Equal(member.Name, value.Name);
            Assert.Equal(member.Email, value.Email);
            Assert.Equal(member.StreetAddress, value.StreetAddress);
            Assert.Equal(member.City, value.City);
            Assert.Equal(member.State, value.State);
            Assert.Equal(member.ZipCode, value.ZipCode);
            Assert.Equal(member.Status, value.Status);
        }

        /// <summary>
        /// Verifies MemberController.GetAsync(id) returns NotFoundResult if repository cannot find member 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAsyncNotFound()
        {
            // * Arrange *

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.GetAsync(It.IsAny<int>()))
                .Returns(Task.FromResult<Member>(null));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.GetAsync(1);

            // * Assert *

            // Verify call returns NotFoundObjectResult
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Verifies MemberController.GetAsync() handles an Exception thrown by repository by: 
        /// 1) logging the exception with the specified message
        /// 2) returning an ObjectResult with StatusCode value of 500
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAsyncException()
        {
            // * Arrange *

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.GetAsync(It.IsAny<int>()))
                .Throws<Exception>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            // Note: any Id value can be used because any 
            // call to the repository will throw an exception
            var result = await controller.GetAsync(1);

            // * Assert *

            // Verify repository's GetAllAsync method was called
            mockRepository.Verify(repository => repository.GetAsync(It.IsAny<int>()), Times.Once());

            // Verify controller returned an ObjectResult whose status code is 500
            Assert.NotNull(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            // Verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.GetAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies MemberController.AddAsync() adds a member to the repository returns an OkObjectResult 
        /// Verifies that an OkObjectResult is returned
        /// Verifies that the OkObjectResult's value property is that of the member
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidatePostAsync()
        {
            // * Arrange *

            var (resource, _) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.AddAsync(It.IsAny<Member>()));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.PostAsync(resource);

            // * Assert *

            // 1) Verify repository's AddAsync method was called once with the specified resource data
            mockRepository.Verify(repository => repository.AddAsync(It.IsAny<Member>()), Times.Once());
            mockRepository.Verify(repository => repository.AddAsync(It.Is<Member>(
                m =>
                m.Id == 0 &&
                m.Name == resource.Name &&
                m.Email == resource.Email &&
                m.StreetAddress == resource.StreetAddress &&
                m.City == resource.City &&
                m.State == resource.State &&
                m.ZipCode == resource.ZipCode &&
                m.Status == resource.Status
                )));

            // 2) Verify controller returned a CreatedResult
            Assert.NotNull(result);
            var createdResult = Assert.IsType<CreatedResult>(result);

            // 3) Verify the returned resource is equivalent to the entered resource
            var value = Assert.IsType<MemberResource>(createdResult.Value);
            Assert.NotNull(value);
            Assert.Equal(resource.Name, value.Name);
            Assert.Equal(resource.Email, value.Email);
            Assert.Equal(resource.StreetAddress, value.StreetAddress);
            Assert.Equal(resource.City, value.City);
            Assert.Equal(resource.State, value.State);
            Assert.Equal(resource.ZipCode, value.ZipCode);
            Assert.Equal(resource.Status, value.Status);
        }

        /// <summary>
        /// Verifies MemberController.PostAsync() handles an Exception thrown by repository by: 
        /// 1) logging the exception with the specified message
        /// 2) returning an ObjectResult with StatusCode value of 500
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidatePostAsyncException()
        {
            // * Arrange *

            var (resource, _) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.AddAsync(It.IsAny<Member>()))
                .Throws<Exception>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.PostAsync(resource);

            // * Assert *

            // Verify repository's GetAllAsync method was called
            // 1) Verify repository's AddAsync method was called once with the specified resource data
            mockRepository.Verify(repository => repository.AddAsync(It.IsAny<Member>()), Times.Once());
            mockRepository.Verify(repository => repository.AddAsync(It.Is<Member>(
                m =>
                m.Id == 0 &&
                m.Name == resource.Name &&
                m.Email == resource.Email &&
                m.StreetAddress == resource.StreetAddress &&
                m.City == resource.City &&
                m.State == resource.State &&
                m.ZipCode == resource.ZipCode &&
                m.Status == resource.Status
                )));

            // 1) Verify controller returned an ObjectResult whose status code is 500

            Assert.NotNull(result);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            // 2) verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.PostAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies MemberController.PutAsync() updates a member in the repository 
        /// Verifies that an OkObjectResult is returned
        /// Verifies that the OkObjectResult's value property is that of the member resource
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidatePutAsync()
        {
            // * Arrange *

            var (resource, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.UpdateAsync(It.IsAny<Member>()))
                .Returns(Task.FromResult(1));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.PutAsync(member.Id, resource);

            // * Assert *

            // Verify repository's AddAsync method was called once with the specified resource data
            mockRepository.Verify(repository => repository.UpdateAsync(It.IsAny<Member>()), Times.Once());
            mockRepository.Verify(repository => repository.UpdateAsync(It.Is<Member>(
                m =>
                m.Id == member.Id &&
                m.Name == member.Name &&
                m.Email == member.Email &&
                m.StreetAddress == member.StreetAddress &&
                m.City == member.City &&
                m.State == member.State &&
                m.ZipCode == member.ZipCode &&
                m.Status == member.Status
                )));

            // Verify controller returned a OkObjectResult
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkResult>(result);
        }

        /// <summary>
        /// Verifies MemberController.PutAsync() handles a DbConcurrencyException thrown by repository by: 
        /// 1) logging the exception with the specified message
        /// 2) returning a BadRequestResult
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidatePutAsyncConcurrencyException()
        {
            // * Arrange *

            var (resource, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.UpdateAsync(It.IsAny<Member>()))
                .Throws<DbUpdateConcurrencyException>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.PutAsync(member.Id, resource);

            // * Assert *

            // Verify repository's UpdateAsync method was called once with the specified resource data
            mockRepository.Verify(repository => repository.UpdateAsync(It.IsAny<Member>()), Times.Once());

            // Verify controller returned an BadRequestResult
            Assert.NotNull(result);
            Assert.IsType<BadRequestResult>(result);

            // Verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.PutAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies MemberController.PutAsync() handles an Exception thrown by repository by: 
        /// 1) logging the exception with the specified message
        /// 2) returning an ObjectResult with StatusCode value of 500
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidatePutAsyncException()
        {
            // * Arrange *

            var (resource, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.UpdateAsync(It.IsAny<Member>()))
                .Throws<Exception>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.PutAsync(member.Id, resource);

            // * Assert *

            // 1) Verify repository's AddAsync method was called once with the specified resource data
            mockRepository.Verify(repository => repository.UpdateAsync(It.IsAny<Member>()), Times.Once());

            // 1) Verify controller returned an ObjectResult whose status code is 500

            Assert.NotNull(result);

            var oResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, oResult.StatusCode);

            // 2) verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.PutAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        /// <summary>
        /// Verifies MemberController.DeleteAsync(id) returns an OkObjectResult if the member is found
        /// Verifies the value of the OkObjectResult is the found member entity
        /// Verifies that the repository was called once to delete the member
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteAsync()
        {
            // * Arrange *

            var (_, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.DeleteAsync(member.Id))
                .Returns(Task.FromResult(member));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.DeleteAsync(member.Id);

            // * Assert *

            // Verify call returns OkObjectResult
            var okObjectResult = Assert.IsType<OkObjectResult>(result);

            // Verify the member returned by OkObjectResult
            var value = Assert.IsType<Member>(okObjectResult.Value);
            Assert.Equal(member.Id, value.Id);
            Assert.Equal(member.Name, value.Name);
            Assert.Equal(member.Email, value.Email);
            Assert.Equal(member.StreetAddress, value.StreetAddress);
            Assert.Equal(member.City, value.City);
            Assert.Equal(member.State, value.State);
            Assert.Equal(member.ZipCode, value.ZipCode);
            Assert.Equal(member.Status, value.Status);
        }
        
        /// <summary>
        /// Verifies MemberController.DeleteAsync(id) returns NotFoundResult if repository cannot find member 
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteAsyncNotFound()
        {
            // * Arrange *

            var (_, member) = CreateResourceAndMember();

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.DeleteAsync(member.Id))
                .Returns(Task.FromResult<Member>(null));

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.DeleteAsync(member.Id);

            // * Assert *

            // Verify call returns OkObjectResult
            Assert.IsType<NotFoundResult>(result);
        }

        /// <summary>
        /// Verifies MemberController.DeleteAsync() handles exception thrown by repository by: 
        /// 1) logging the exception with the specified message
        /// 2) returning an ObjectResult with StatusCode value of 500
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task DeleteAsyncException()
        {
            // * Arrange *

            var mockLogger = new Mock<ILogger<MemberController>>();

            var mockRepository = new Mock<IRepository<Member>>();
            mockRepository
                .Setup(repository => repository.DeleteAsync(It.IsAny<int>()))
                .Throws<Exception>();

            var mockDefaultPagingOptions = new Mock<IOptions<PagingOptions>>();
            mockDefaultPagingOptions
                .Setup(defaultPagingOptions => defaultPagingOptions.Value)
                .Returns(new PagingOptions() { Offset = 0, Limit = 3 });

            // * Act *

            var controller = new MemberController(mockLogger.Object, mockRepository.Object, mockDefaultPagingOptions.Object);
            var result = await controller.DeleteAsync(1);

            // * Assert *

            // Verify repository's GetAllAsync method was called
            mockRepository.Verify(repository => repository.DeleteAsync(It.IsAny<int>()), Times.Once());

            // Verify controller returned an ObjectResult whose status code is 500
            Assert.NotNull(result);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, objectResult.StatusCode);

            // Verify exception was logged
            mockLogger.Verify(logger => logger.Log(
                It.Is<LogLevel>(logLevel => logLevel == LogLevel.Error),
                It.Is<EventId>(eventId => eventId.Id == 0),
                It.Is<It.IsAnyType>((@object, @type) => @object.ToString() == nameof(MemberController.DeleteAsync) && @type.Name == "FormattedLogValues"),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}