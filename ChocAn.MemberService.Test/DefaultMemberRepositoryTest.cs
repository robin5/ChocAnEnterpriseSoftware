// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultMemberRepositoryTest.cs
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

using ChocAn.GenericRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace ChocAn.MemberRepository.Test
{
    /// <summary>
    /// Tests for DefaultMemberRepository class
    /// </summary>
    public class DefaultMemberRepositoryTest
    {
        #region Useful Constants

        // Note: All constants need be unique
        private const string NON_EXISTENT_MEMBER_ID = "5bd1ffc8-9047-4219-b9a2-ae2e25db8118";

        private const string VALID0_ID = "5bd1ffc8-9047-4219-86f0-ee93777ca57e";
        private const decimal VALID0_NUMBER = 999999999;
        private const string VALID0_NAME = "1234567890123456789012345";
        private const string VALID0_ADDRESS = "1234567890123456789012345";
        private const string VALID0_CITY = "12345678901234";
        private const string VALID0_STATE = "12";
        private const decimal VALID0_ZIPCODE = 99999;
        private const string VALID0_STATUS = "Status 0";

        private const string VALID1_ID = "3b3170f8-963d-4729-a687-ae2e25db7bb7";
        private const decimal VALID1_NUMBER = 1;
        private const string VALID1_NAME = "Name 1";
        private const string VALID1_ADDRESS = "Address 1";
        private const string VALID1_CITY = "City 1";
        private const string VALID1_STATE = "WA";
        private const decimal VALID1_ZIPCODE = 20001;
        private const string VALID1_STATUS = "Status 1";

        private const string VALID2_ID = "dec047b3-1918-40bd-b9a2-f137a44429a5";
        private const decimal VALID2_NUMBER = 2;
        private const string VALID2_NAME = "Name 2";
        private const string VALID2_ADDRESS = "Address 2";
        private const string VALID2_CITY = "City 2";
        private const string VALID2_STATE = "OR";
        private const decimal VALID2_ZIPCODE = 30002;
        private const string VALID2_STATUS = "Status 2";

        private const decimal VALID_UPDATE_NUMBER = 100000009;
        private const string VALID_UPDATE_NAME = "1234567890";
        private const string VALID_UPDATE_ADDRESS = "1234567890123";
        private const string VALID_UPDATE_CITY = "1232345";
        private const string VALID_UPDATE_STATE = "CA";
        private const decimal VALID_UPDATE_ZIPCODE = 10026;
        private const string VALID_UPDATE_STATUS = "suspended";
        #endregion

        /// <summary>
        /// Creates an instance of an InMemory database
        /// </summary>
        /// <param name="name">Name of InMemory database instance</param>
        /// <returns></returns>
        private static MemberDbContext GetContext(string name)
        {
            var dbOptions = new DbContextOptionsBuilder<MemberDbContext>()
                .UseInMemoryDatabase(name).Options;

            return new MemberDbContext(dbOptions);
        }

        /// <summary>
        /// Inserts 1 valid member into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task InsertValidMemberIntoTestDatabase(string name)
        {
            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext(name))
            {
                // Arrange
                var member = new Member
                {
                    Id = new Guid(VALID0_ID),
                    Number = VALID0_NUMBER,
                    Name = VALID0_NAME,
                    StreetAddress = VALID0_ADDRESS,
                    City = VALID0_CITY,
                    State = VALID0_STATE,
                    ZipCode = VALID0_ZIPCODE,
                    Status = VALID0_STATUS
                };

                context.Add<Member>(member);
                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Inserts 3 valid members into the test database
        /// </summary>
        /// <param name="name">Name of InMemory database instance to use</param>
        /// <returns></returns>
        private static async Task Insert3ValidMembersIntoTestDatabase(string name)
        {
            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext(name))
            {
                // Arrange
                context.Add<Member>(new Member
                {
                    Id = new Guid(VALID0_ID),
                    Number = VALID0_NUMBER,
                    Name = VALID0_NAME,
                    StreetAddress = VALID0_ADDRESS,
                    City = VALID0_CITY,
                    State = VALID0_STATE,
                    ZipCode = VALID0_ZIPCODE,
                    Status = VALID0_STATUS
                });
                context.Add<Member>(new Member
                {
                    Id = new Guid(VALID1_ID),
                    Number = VALID1_NUMBER,
                    Name = VALID1_NAME,
                    StreetAddress = VALID1_ADDRESS,
                    City = VALID1_CITY,
                    State = VALID1_STATE,
                    ZipCode = VALID1_ZIPCODE,
                    Status = VALID1_STATUS
                });
                context.Add<Member>(new Member
                {
                    Id = new Guid(VALID2_ID),
                    Number = VALID2_NUMBER,
                    Name = VALID2_NAME,
                    StreetAddress = VALID2_ADDRESS,
                    City = VALID2_CITY,
                    State = VALID2_STATE,
                    ZipCode = VALID2_ZIPCODE,
                    Status = VALID2_STATUS
                });

                await context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Verifies Inserting a member into the database
        /// </summary>
        [Fact]
        public async Task ValidateAddAsync()
        {
            // Arrange
            Guid validId = new Guid(VALID0_ID);

            var member = new Member
            {
                Id = validId,
                Number = VALID0_NUMBER,
                Name = VALID0_NAME,
                StreetAddress = VALID0_ADDRESS,
                City = VALID0_CITY,
                State = VALID0_STATE,
                ZipCode = VALID0_ZIPCODE,
                Status = VALID0_STATUS
            };

            // Act
            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("Add"))
            {
                var defaultMemberService = new DefaultMemberRepository(context);
                var result = await defaultMemberService.AddAsync(member);
            }

            // Assert
            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("Add"))
            {
                var result = context.Find<Member>(validId);

                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_NUMBER, result.Number);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
                Assert.Equal(VALID0_CITY, result.City);
                Assert.Equal(VALID0_STATE, result.State);
                Assert.Equal(VALID0_ZIPCODE, result.ZipCode);
                Assert.Equal(VALID0_STATUS, result.Status);
            }
        }

        /// <summary>
        /// Verifies getting a member from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetMemberAsync()
        {
            // Arrange
            await DefaultMemberRepositoryTest.InsertValidMemberIntoTestDatabase("Get");
            var validId = new Guid(VALID0_ID);

            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("Get"))
            {
                // Act
                var defaultMemberService = new DefaultMemberRepository(context);
                var result = await defaultMemberService.GetAsync(validId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID0_NUMBER, result.Number);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
                Assert.Equal(VALID0_CITY, result.City);
                Assert.Equal(VALID0_STATE, result.State);
                Assert.Equal(VALID0_ZIPCODE, result.ZipCode);
                Assert.Equal(VALID0_STATUS, result.Status);
            }
        }

        /// <summary>
        /// Verifies getting a nonexistent member from the database returns null
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetMemberAsyncNonExistentMember()
        {
            // Arrange
            await DefaultMemberRepositoryTest.InsertValidMemberIntoTestDatabase("ValidateGetMemberAsyncNonExistentMember");
            var nonExistentMemberId = new Guid(NON_EXISTENT_MEMBER_ID);

            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("ValidateGetMemberAsyncNonExistentMember"))
            {
                // Act
                var defaultMemberService = new DefaultMemberRepository(context);
                var result = await defaultMemberService.GetAsync(nonExistentMemberId);

                // Assert
                Assert.Null(result);
            }
        }

        /// <summary>
        /// Verifies updating a member in the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateUpdateAsync()
        {
            // Arrange
            await DefaultMemberRepositoryTest.InsertValidMemberIntoTestDatabase("Update");

            var validId = new Guid(VALID0_ID);

            var memberChanges = new Member
            {
                Id = validId,
                Number = VALID_UPDATE_NUMBER,
                Name = VALID_UPDATE_NAME,
                StreetAddress = VALID_UPDATE_ADDRESS,
                City = VALID_UPDATE_CITY,
                State = VALID_UPDATE_STATE,
                ZipCode = VALID_UPDATE_ZIPCODE,
                Status = VALID_UPDATE_STATUS
            };

            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("Update"))
            {
                // Act
                var defaultMemberService = new DefaultMemberRepository(context);
                var result = await defaultMemberService.UpdateAsync(memberChanges);

                // Assert
                // Validate return value of function call
                Assert.NotNull(result);
                Assert.Equal(validId, result.Id);
                Assert.Equal(VALID_UPDATE_NUMBER, result.Number);
                Assert.Equal(VALID_UPDATE_NAME, result.Name);
                Assert.Equal(VALID_UPDATE_ADDRESS, result.StreetAddress);
                Assert.Equal(VALID_UPDATE_CITY, result.City);
                Assert.Equal(VALID_UPDATE_STATE, result.State);
                Assert.Equal(VALID_UPDATE_ZIPCODE, result.ZipCode);
                Assert.Equal(VALID_UPDATE_STATUS, result.Status);

                // Validate member was updated in the database
                var member = await context.Members.FindAsync(validId);
                Assert.NotNull(member);
                Assert.Equal(validId, member.Id);
                Assert.Equal(VALID_UPDATE_NUMBER, member.Number);
                Assert.Equal(VALID_UPDATE_NAME, member.Name);
                Assert.Equal(VALID_UPDATE_ADDRESS, member.StreetAddress);
                Assert.Equal(VALID_UPDATE_CITY, member.City);
                Assert.Equal(VALID_UPDATE_STATE, member.State);
                Assert.Equal(VALID_UPDATE_ZIPCODE, member.ZipCode);
                Assert.Equal(VALID_UPDATE_STATUS, member.Status);
            }
        }

        /// <summary>
        /// Verifies deleting a member from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateDeleteAsync()
        {
            // Arrange
            await DefaultMemberRepositoryTest.InsertValidMemberIntoTestDatabase("Delete");

            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("Delete"))
            {
                // Act
                var defaultMemberService = new DefaultMemberRepository(context);
                var result = await defaultMemberService.DeleteAsync(new Guid(VALID0_ID));

                // Assert
                Assert.NotNull(result);
                Assert.Equal(VALID0_ID, result.Id.ToString());
                Assert.Equal(VALID0_NUMBER, result.Number);
                Assert.Equal(VALID0_NAME, result.Name);
                Assert.Equal(VALID0_ADDRESS, result.StreetAddress);
                Assert.Equal(VALID0_CITY, result.City);
                Assert.Equal(VALID0_STATE, result.State);
                Assert.Equal(VALID0_ZIPCODE, result.ZipCode);
                Assert.Equal(VALID0_STATUS, result.Status);

                Assert.Equal(0, await context.Members.CountAsync());
            }
        }

        /// <summary>
        /// Verifies getting all members from the database
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task ValidateGetAllMembersAsync()
        {
            // Arrange
            await DefaultMemberRepositoryTest.Insert3ValidMembersIntoTestDatabase("GetAllMembersAsync");

            bool member0Found = false;
            bool member1Found = false;
            bool member2Found = false;

            using (MemberDbContext context = DefaultMemberRepositoryTest.GetContext("GetAllMembersAsync"))
            {
                // Act
                var defaultMemberService = new DefaultMemberRepository(context);

                // Assert
                await foreach (Member member in defaultMemberService.GetAllAsync())
                {
                    if (VALID0_ID == member.Id.ToString())
                    {
                        Assert.Equal(VALID0_NUMBER, member.Number);
                        Assert.Equal(VALID0_NAME, member.Name);
                        Assert.Equal(VALID0_ADDRESS, member.StreetAddress);
                        Assert.Equal(VALID0_CITY, member.City);
                        Assert.Equal(VALID0_STATE, member.State);
                        Assert.Equal(VALID0_ZIPCODE, member.ZipCode);
                        Assert.Equal(VALID0_STATUS, member.Status);
                        member0Found = true;
                    }
                    else if (VALID1_ID == member.Id.ToString())
                    {
                        Assert.Equal(VALID1_NUMBER, member.Number);
                        Assert.Equal(VALID1_NAME, member.Name);
                        Assert.Equal(VALID1_ADDRESS, member.StreetAddress);
                        Assert.Equal(VALID1_CITY, member.City);
                        Assert.Equal(VALID1_STATE, member.State);
                        Assert.Equal(VALID1_ZIPCODE, member.ZipCode);
                        Assert.Equal(VALID1_STATUS, member.Status);
                        member1Found = true;
                    }
                    else if (VALID2_ID == member.Id.ToString())
                    {
                        Assert.Equal(VALID2_NUMBER, member.Number);
                        Assert.Equal(VALID2_NAME, member.Name);
                        Assert.Equal(VALID2_ADDRESS, member.StreetAddress);
                        Assert.Equal(VALID2_CITY, member.City);
                        Assert.Equal(VALID2_STATE, member.State);
                        Assert.Equal(VALID2_ZIPCODE, member.ZipCode);
                        Assert.Equal(VALID2_STATUS, member.Status);
                        member2Found = true;
                    }
                }

                // There should be 3 members in the database
                Assert.True(member0Found);
                Assert.True(member1Found);
                Assert.True(member2Found);
            }
        }
    }
}
