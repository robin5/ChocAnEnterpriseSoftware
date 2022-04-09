// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockRepository.cs
// *
// * Description: Mocks a generic repoitory class
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChocAn.Repository.Paging;

namespace ChocAn.MockRepositories
{
    public class MockRepository<T>
    {
        protected List<T> items = new();

        public Task<T> AddAsync(T item)
        {
            items.Add(item);
            return Task.FromResult(item);
        }

        public Task<T> DeleteAsync(object id)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<T> GetAllAsync(PagingOptions pagingOptions)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            T item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current))
            {
                yield return item;
                //await Task.Delay(1);
                await Task.FromResult(enumerator.MoveNext());
            }
            // return Task.FromResult(items.AsEnumerable<T>());
        }

        public Task<T> GetAsync(object id)
        {
            throw new NotImplementedException();
        }

        public Task<T> UpdateAsync(T memberChanges)
        {
            throw new NotImplementedException();
        }

        public async IAsyncEnumerable<T> GetAllByNameAsync(string name)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            T item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current))
            {
                yield return item;
                //await Task.Delay(1);
                await Task.FromResult(enumerator.MoveNext());
            }
            // return Task.FromResult(items.AsEnumerable<T>());
        }
    }
}