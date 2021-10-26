// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ITransactionRepository.cs
// *
// * Description: Defines an interface for storing Transaction objects in a database
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

namespace ChocAn.TransactionRepository
{
    /// <summary>
    /// Represents repository pattern for Transaction entities
    /// </summary>
    public interface ITransactionRepository
    {
        /// <summary>
        /// Adds a Transaction entity to the database
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        Task<Transaction> AddAsync(Transaction transaction);

        /// <summary>
        /// Retrieves a Transaction entity from the database
        /// </summary>
        /// <param name="id">ID of Transaction entity to retrieve</param>
        /// <returns></returns>
        Task<Transaction> GetAsync(Guid id);

        /// <summary>
        /// Updates a Transaction entity in the database
        /// </summary>
        /// <param name="transactionChanges">Changes to be applied to Transaction entity</param>
        /// <returns></returns>
        Task<Transaction> UpdateAsync(Transaction transactionChanges);

        /// <summary>
        /// Deletes Transaction entity from the database
        /// </summary>
        /// <param name="id">ID of transaction to deleted</param>
        /// <returns></returns>
        Task<Transaction> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves all Transaction entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Transaction Entities in the database</returns>
        IAsyncEnumerable<Transaction> GetAllAsync();
    }
}
