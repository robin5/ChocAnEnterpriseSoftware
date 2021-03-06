// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockTransactionRepository.cs
// *
// * Description: mocks the TransactionService class
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
// **********************************************************************************using System;

using System;
using System.Collections.Generic;
using ChocAn.Data;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Search;
using ChocAn.Repository.Sorting;
using ChocAn.TransactionRepository;

namespace ChocAn.MockRepositories
{
    public class MockTransactionRepository :
        MockRepository<Transaction>,
        ITransactionRepository
    {
        public IAsyncEnumerable<Transaction> GetAllAsync(PagingOptions pagingOptions, SortOptions<Transaction> sortOptions, SearchOptions<Transaction> searchOptions)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<Transaction> ITransactionRepository.GetAccountsPayableTransactionsAsync(DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<Transaction> ITransactionRepository.GetMemberTransactionsAsync(int memberId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }

        IAsyncEnumerable<Transaction> ITransactionRepository.GetProviderTransactionsAsync(int providerId, DateTime startDate, DateTime endDate)
        {
            throw new NotImplementedException();
        }
    }
}
