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
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository;
using System.Threading.Tasks;

namespace ChocAn.TransactionRepository
{
    /// <summary>
    /// Implements repository pattern for Transaction entities
    /// </summary>
    public class DefaultTransactionRepository : Repository<Transaction>, ITransactionRepository
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
        /// Adds a transaction to the database and sets the created DateTime to now
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        override public async Task<Transaction> AddAsync(Transaction obj)
        {
            obj.Created = DateTime.Now;
            await dbSet.AddAsync(obj);
            context.SaveChanges();
            return obj;
        }
        /// <summary>
        /// Returns transactions of member specified by memberId between start and end dates
        /// </summary>
        /// <param name="memberId">ID of member</param>
        /// <param name="startDate">Start date of transactions</param>
        /// <param name="endDate">End date of transactions</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Transaction> GetMemberTransactionsAsync(int memberId, DateTime startDate, DateTime endDate)
        {
            var query = dbSet.Where<Transaction>(a =>
                (a.MemberId == memberId) &&
                (a.ServiceDate >= startDate) &&
                (a.ServiceDate <= endDate));

            var enumerator = query.AsAsyncEnumerable<Transaction>().GetAsyncEnumerator();
            Transaction transaction;

            await enumerator.MoveNextAsync();
            while (null != (transaction = enumerator.Current))
            {
                yield return transaction;
                await enumerator.MoveNextAsync();
            }
        }
        /// <summary>
        /// Returns transactions of provider specified by providerId between start and end dates
        /// </summary>
        /// <param name="providerId">ID of Provider</param>
        /// <param name="startDate">Start date of transactions</param>
        /// <param name="endDate">End date of transactions</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Transaction> GetProviderTransactionsAsync(int providerId, DateTime startDate, DateTime endDate)
        {
            var query = dbSet.Where<Transaction>(a =>
                (a.ProviderId == providerId) &&
                (a.ServiceDate >= startDate) &&
                (a.ServiceDate <= endDate));

            var enumerator = query.AsAsyncEnumerable<Transaction>().GetAsyncEnumerator();
            Transaction transaction;

            await enumerator.MoveNextAsync();
            while (null != (transaction = enumerator.Current))
            {
                yield return transaction;
                await enumerator.MoveNextAsync();
            }
        }
        /// <summary>
        /// Returns all transactions between start and end dates
        /// </summary>
        /// <param name="startDate">Start date of transactions</param>
        /// <param name="endDate">End date of transactions</param>
        /// <returns></returns>
        public async IAsyncEnumerable<Transaction> GetAccountsPayableTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            var transactionQuery = dbSet.Where<Transaction>(t => (t.ServiceDate >= startDate) && (t.ServiceDate <= endDate));
            var transactions = transactionQuery.AsAsyncEnumerable<Transaction>().GetAsyncEnumerator();
            Transaction transaction;

            await transactions.MoveNextAsync();
            while (null != (transaction = transactions.Current))
            {
                yield return transaction;
                await transactions.MoveNextAsync();
            }

            #region save
            //var reportItems = new List<MemberReportItem>();
            /*
            IEnumerable<IGrouping<int, Transaction>> query1 =
                from transaction in dbSet
                where (transaction.ServiceDate >= startDate) && (transaction.ServiceDate <= endDate)
                group transaction by transaction.ProviderId;
            */
            /*
            var transactions = dbSet
                .Where(a => ((a.ServiceDate >= startDate) && (a.ServiceDate <= endDate)))
                .AsEnumerable()
                .GroupBy(a => a.ProviderId);
            */
            #endregion

            // yield return dbSet
            //     .Where(a => ((a.ServiceDate >= startDate) && (a.ServiceDate <= endDate))).ToListAsync();



            #region save
            /*

            var enumerator = transactions.AsAsyncEnumerable().GetAsyncEnumerator();

            try
            {
                foreach(var provider in transactions)
                {
                    decimal totalFee = 0;
                    decimal transactionCount = 0;
                    
                    foreach (Transaction t in provider)
                    {
                        totalFee += t.ProductCost;
                        transactionCount++;
                        Console.WriteLine("    Member:{0} Service:{1}", t.MemberId, t.ServiceId);
                    }
                    reportItems.Add(new MemberReportItem { 
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            */
            /*
            var query = dbSet.Where<Transaction>(a =>
                (a.ServiceDate >= startDate) &&
                (a.ServiceDate <= endDate));

            var enumerator = transactions.AsAsyncEnumerable<Transaction>().GetAsyncEnumerator();

            IGrouping<int, Transaction> current;

            await enumerator.MoveNextAsync();
            while (null != (current = enumerator.Current))
            {
                yield return current;
                await enumerator.MoveNextAsync();
            }
            */
            #endregion
        }
    }
}
