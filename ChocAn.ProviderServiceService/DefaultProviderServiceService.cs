// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProviderServiceService.cs
// *
// * Description: The DefaultProviderServiceService class provides access to ProviderService items
// *              stored in a database context
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

namespace ChocAn.ProviderServiceService
{
    /// <summary>
    /// Implements repository pattern for ProviderService entities
    /// </summary>
    public class DefaultProviderServiceService : IProviderServiceService
    {
        private readonly ProviderServiceDbContext context;

        /// <summary>
        ///  Constructor for ProviderServiceDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultProviderServiceService(ProviderServiceDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a ProviderService entity to the database
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public async Task<ProviderService> AddAsync(ProviderService member)
        {
            await context.ProviderServices.AddAsync(member);
            context.SaveChanges();
            return member;
        }

        /// <summary>
        /// Retrieves a ProviderService entity from the database
        /// </summary>
        /// <param name="id">ID of ProviderService entity to retrieve</param>
        /// <returns></returns>
        public async Task<ProviderService> GetAsync(Guid id)
        {
            return await context.ProviderServices.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a ProviderService entity from the database by service code
        /// </summary>
        /// <param name="code">Code of ProviderService entity to retrieve</param>
        /// <returns></returns>
        public async Task<ProviderService> GetByCodeAsync(decimal code)
        {
            return await context.ProviderServices.Where(p => p.Code == code).FirstOrDefaultAsync<ProviderService>();
        }

        /// <summary>
        /// Updates a ProviderService entity in the database
        /// </summary>
        /// <param name="memberChanges">Changes to be applied to ProviderService entity</param>
        /// <returns></returns>
        public async Task<ProviderService> UpdateAsync(ProviderService memberChanges)
        {
            var member = context.ProviderServices.Attach(memberChanges);
            member.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return memberChanges;
        }

        /// <summary>
        /// Deletes ProviderService entity from the database
        /// </summary>
        /// <param name="id">ID of member to deleted</param>
        /// <returns></returns>
        public async Task<ProviderService> DeleteAsync(Guid id)
        {
            var member = await context.ProviderServices.FindAsync(id);
            if (null != member)
            {
                context.ProviderServices.Remove(member);
                await context.SaveChangesAsync();
            }
            return member;
        }

        /// <summary>
        /// Retrieves all ProviderService entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all ProviderService Entities in the database</returns>
        public async IAsyncEnumerable<ProviderService> GetAllAsync()
        {
            var enumerator = context.ProviderServices.AsAsyncEnumerable().GetAsyncEnumerator();
            ProviderService member;

            await enumerator.MoveNextAsync();
            while (null != (member = enumerator.Current))
            {
                yield return member;
                await enumerator.MoveNextAsync();
            }
        }
    }
}
