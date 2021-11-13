// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockProviderRepository.cs
// *
// * Description: Mocks the ProviderService class
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

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChocAn.ProviderRepository;

namespace ChocAn.MockRepositories
{
    public class MockProviderRepository : IProviderRepository
    {
        protected IDictionary<decimal, Provider> items = new Dictionary<decimal, Provider>();
        decimal key = 0;

        public Task<Provider> AddAsync(Provider item)
        {
            items.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<Provider> DeleteAsync(object id)
        {
            Provider item = null;

            if (items.TryGetValue((decimal)id, out item))
            {
                items.Remove((decimal)id);
                return Task.FromResult(item);
            }
            return Task.FromResult((Provider)null);
        }

        public async IAsyncEnumerable<Provider> GetAllAsync()
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Provider item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current.Value))
            {
                yield return item;
                await Task.FromResult(enumerator.MoveNext());
            }
        }

        public Task<Provider> GetAsync(object id)
        {
            Provider item = null;

            items.TryGetValue((decimal)id, out item);
            return Task.FromResult(item);
        }

        public Task<Provider> UpdateAsync(Provider changes)
        {
            items[changes.Id] = changes;
            return Task.FromResult(changes);
        }

        public async IAsyncEnumerable<Provider> FindAllByNameAsync(string name)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Provider item;

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
