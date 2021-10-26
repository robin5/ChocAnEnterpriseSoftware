// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultProviderRepository.cs
// *
// * Description: Provides access to Provider entities stored in a database context
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

namespace ChocAn.ProviderRepository
{
    /// <summary>
    /// Implements repository pattern for Provider entities
    /// </summary>
    public class DefaultProviderRepository : IProviderRepository
    {
        private readonly ProviderDbContext context;

        /// <summary>
        ///  Constructor for ProviderDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultProviderRepository(ProviderDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a Provider entity to the database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        public async Task<Provider> AddAsync(Provider provider)
        {
            await context.Providers.AddAsync(provider);
            context.SaveChanges();
            return provider;
        }

        /// <summary>
        /// Retrieves a Provider entity from the database
        /// </summary>
        /// <param name="id">ID of Provider entity to retrieve</param>
        /// <returns></returns>
        public async Task<Provider> GetAsync(Guid id)
        {
            return await context.Providers.FindAsync(id);
        }

        /// <summary>
        /// Retrieves a Provider entity from the database by provider number
        /// </summary>
        /// <param name="id">ID of Provider entity to retrieve</param>
        /// <returns></returns>
        public async Task<Provider> GetByNumberAsync(decimal number)
        {
            return await context.Providers.Where(p => p.Number == number).FirstOrDefaultAsync<Provider>();
        }

        /// <summary>
        /// Updates a Provider entity in the database
        /// </summary>
        /// <param name="providerChanges">Changes to be applied to Provider entity</param>
        /// <returns></returns>
        public async Task<Provider> UpdateAsync(Provider providerChanges)
        {
            var provider = context.Providers.Attach(providerChanges);
            provider.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return providerChanges;
        }

        /// <summary>
        /// Deletes Provider entity from the database
        /// </summary>
        /// <param name="id">ID of provider to deleted</param>
        /// <returns></returns>
        public async Task<Provider> DeleteAsync(Guid id)
        {
            var provider = await context.Providers.FindAsync(id);
            if (null != provider)
            {
                context.Providers.Remove(provider);
                await context.SaveChangesAsync();
            }
            return provider;
        }

        /// <summary>
        /// Retrieves all Provider entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Provider Entities in the database</returns>
        public async IAsyncEnumerable<Provider> GetAllProvidersAsync()
        {
            var enumerator = context.Providers.AsAsyncEnumerable().GetAsyncEnumerator();
            Provider provider;

            await enumerator.MoveNextAsync();
            while (null != (provider = enumerator.Current))
            {
                yield return provider;
                await enumerator.MoveNextAsync();
            }
        }
    }
}
