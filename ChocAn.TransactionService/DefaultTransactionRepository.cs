// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: DefaultTransactionRepository.cs
// *
// * Description: Provides access to Transaction items stored in a database context
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
using ChocAn.GenericRepository;
using System.Threading.Tasks;

namespace ChocAn.TransactionRepository
{
    /// <summary>
    /// Implements repository pattern for Transaction entities
    /// </summary>
    public class DefaultTransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        /// <summary>
        ///  Constructor for DefaultTransactionRepository
        /// </summary>
        /// <param name="context">DbContext of underlying database</param>
        public DefaultTransactionRepository(TransactionDbContext context)
            : base(context)
        {
        }
        /// <summary>
        /// Adds entity to the database and sets TransactionDateTime to now
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        override public async Task<Transaction> AddAsync(Transaction obj)
        {
            obj.TransactionDateTime = DateTime.Now;
            await dbSet.AddAsync(obj);
            context.SaveChanges();
            return obj;
        }
    }
}
