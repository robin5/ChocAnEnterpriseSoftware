// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultTransactionService.cs
// *
// * Description: The DefaultTransactionService class provides access to Transaction items
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

namespace ChocAn.TransactionService
{
    /// <summary>
    /// Implements repository pattern for Transaction entities
    /// </summary>
    public class DefaultTransactionService : ITransactionService
    {
        private readonly TransactionDbContext context;

        /// <summary>
        ///  Constructor for TransactionDbContext
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultTransactionService(TransactionDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Adds a Transaction entity to the database
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            await context.Transactions.AddAsync(transaction);
            context.SaveChanges();
            return transaction;
        }

        /// <summary>
        /// Retrieves a Transaction entity from the database
        /// </summary>
        /// <param name="id">ID of Transaction entity to retrieve</param>
        /// <returns></returns>
        public async Task<Transaction> GetAsync(Guid id)
        {
            return await context.Transactions.FindAsync(id);
        }

        /// <summary>
        /// Updates a Transaction entity in the database
        /// </summary>
        /// <param name="transactionChanges">Changes to be applied to Transaction entity</param>
        /// <returns></returns>
        public async Task<Transaction> UpdateAsync(Transaction transactionChanges)
        {
            var transaction = context.Transactions.Attach(transactionChanges);
            transaction.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await context.SaveChangesAsync();
            return transactionChanges;
        }

        /// <summary>
        /// Deletes Transaction entity from the database
        /// </summary>
        /// <param name="id">ID of transaction to deleted</param>
        /// <returns></returns>
        public async Task<Transaction> DeleteAsync(Guid id)
        {
            var transaction = await context.Transactions.FindAsync(id);
            if (null != transaction)
            {
                context.Transactions.Remove(transaction);
                await context.SaveChangesAsync();
            }
            return transaction;
        }

        /// <summary>
        /// Retrieves all Transaction entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Transaction Entities in the database</returns>
        public async IAsyncEnumerable<Transaction> GetAllAsync()
        {
            var enumerator = context.Transactions.AsAsyncEnumerable().GetAsyncEnumerator();
            Transaction transaction;

            await enumerator.MoveNextAsync();
            while (null != (transaction = enumerator.Current))
            {
                yield return transaction;
                await enumerator.MoveNextAsync();
            }
        }
    }
}
