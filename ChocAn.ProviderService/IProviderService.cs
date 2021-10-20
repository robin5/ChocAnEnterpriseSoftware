// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: IProviderService.cs
// *
// * Description: IProviderService defines an interface for storing Provider objects
// *              in a database
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

namespace ChocAn.ProviderService
{
    /// <summary>
    /// Represents repository pattern for Provider entities
    /// </summary>
    public interface IProviderService
    {
        /// <summary>
        /// Adds a Provider entity to the database
        /// </summary>
        /// <param name="provider"></param>
        /// <returns></returns>
        Task<Provider> AddAsync(Provider provider);

        /// <summary>
        /// Retrieves a Provider entity from the database
        /// </summary>
        /// <param name="id">ID of Provider entity to retrieve</param>
        /// <returns></returns>
        Task<Provider> GetProviderAsync(Guid id);

        /// <summary>
        /// Updates a Provider entity in the database
        /// </summary>
        /// <param name="providerChanges">Changes to be applied to Provider entity</param>
        /// <returns></returns>
        Task<Provider> UpdateAsync(Provider providerChanges);

        /// <summary>
        /// Deletes Provider entity from the database
        /// </summary>
        /// <param name="id">ID of provider to deleted</param>
        /// <returns></returns>
        Task<Provider> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves all Provider entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Provider Entities in the database</returns>
        IAsyncEnumerable<Provider> GetAllProvidersAsync();
    }
}
