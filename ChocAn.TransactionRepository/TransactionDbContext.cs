// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Transaction.cs
// *
// * Description: The TransactionDbContext class defines a DbContext for Transaction 
// *              entities
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

using Microsoft.EntityFrameworkCore;
using ChocAn.Data;

namespace ChocAn.TransactionRepository
{
    /// <summary>
    /// A TransactionDbContext instance represents a session with the database and can be used to
    /// query and save instances of Transaction entities. 
    /// </summary>
    public class TransactionDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the TransactionDbContext class
        /// </summary>
        /// <param name="options">The options to be used by the DbContext</param>
        public TransactionDbContext(DbContextOptions<TransactionDbContext> options)
            : base(options)
        {
        }
        /// <summary>
        /// Provides access to Transaction entities in the database
        /// </summary>
        public DbSet<Transaction> Transactions { get; set; }
    }
}
