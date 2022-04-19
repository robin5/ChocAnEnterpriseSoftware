// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockProductRepository.cs
// *
// * Description: Mocks the ProductRepository class
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

using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ChocAn.Repository;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;
using ChocAn.ProductRepository;

namespace ChocAn.MockRepositories
{
    public class MockProductRepository : IRepository<Product>
    {
        protected IDictionary<int, Product> items = new Dictionary<int, Product>();

        public Task<Product> AddAsync(Product item)
        {
            items.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<Product> DeleteAsync(object id)
        {
            if (items.TryGetValue((int)id, out Product item))
            {
                items.Remove((int)id);
                return Task.FromResult(item);
            }
            return Task.FromResult((Product)null);
        }

        public async IAsyncEnumerable<Product> GetAllAsync(
            PagingOptions pagingOptions, 
            SortOptions<Product> sortOptions,
            SearchOptions<Product> searchOptions)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Product item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current.Value))
            {
                yield return item;
                await Task.FromResult(enumerator.MoveNext());
            }
        }

        public Task<Product> GetAsync(object id)
        {
            items.TryGetValue((int)id, out Product item);
            return Task.FromResult(item);
        }

        public Task<int> UpdateAsync(Product changes)
        {
            items[changes.Id] = changes;
            return Task.FromResult(1);
        }

        public async IAsyncEnumerable<Product> GetAllByNameAsync(string name)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Product item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current.Value))
            {
                if (!item.Name.Contains(name)) continue;
                yield return item;
                await Task.FromResult(enumerator.MoveNext());
            }
        }
    }
}
