// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultMemberRepository.cs
// *
// * Description: Provides access to Member entities stored in a database context
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
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ChocAn.MemberRepository
{
    /// <summary>
    /// Implements repository pattern for Member entities
    /// </summary>
    public class DefaultMemberRepository : IMemberRepository
    {
        private readonly MemberDbContext context;

        /// <summary>
        ///  Constructor for MemberDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultMemberRepository(MemberDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a Member entity to the database
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<Member> AddAsync(Member member)
        {
            await context.Members.AddAsync(member);
            context.SaveChanges();
            return member;
        }

        /// <summary>
        /// Retrieves a Member entity from the database
        /// </summary>
        /// <param name="id">ID of Member entity to retrieve</param>
        /// <returns></returns>
        public async Task<Member> GetAsync(Guid id)
        {
            return await context.Members.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a Member entity from the database by member number
        /// </summary>
        /// <param name="id">ID of Member entity to retrieve</param>
        /// <returns></returns>
        public async Task<Member> GetByNumberAsync(decimal number)
        {
            return await context.Members.Where(p => p.Number == number).FirstOrDefaultAsync<Member>();
        }

        /// <summary>
        /// Updates a Member entity in the database
        /// </summary>
        /// <param name="memberChanges">Changes to be applied to Member entity</param>
        /// <returns></returns>
        public async Task<Member> UpdateAsync(Member memberChanges)
        {
            var member = context.Members.Attach(memberChanges);
            member.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return memberChanges;
        }

        /// <summary>
        /// Deletes Member entity from the database
        /// </summary>
        /// <param name="id">ID of member to deleted</param>
        /// <returns></returns>
        public async Task<Member> DeleteAsync(Guid id)
        {
            var member = await context.Members.FindAsync(id);
            if(null != member)
            {
                context.Members.Remove(member);
                await context.SaveChangesAsync();
            }
            return member;
        }

        /// <summary>
        /// Retrieves all Member entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Member Entities in the database</returns>
        public async IAsyncEnumerable<Member> GetAllAsync()
        {
            var enumerator = context.Members.AsAsyncEnumerable().GetAsyncEnumerator();
            Member member;

            await enumerator.MoveNextAsync();
            while (null != (member = enumerator.Current))
            {
                yield return member;
                await enumerator.MoveNextAsync();
            }
        }
    }
}
