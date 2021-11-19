// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockProviderRepository.cs
// *
// * Description: Mocks the ProviderServiceService class
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
using ChocAn.Repository;
using ChocAn.ProviderServiceRepository;

namespace ChocAn.MockRepositories
{
    public class MockProviderServiceRepository : IRepository<ProviderService>
    {
        protected IDictionary<int, ProviderService> items = new Dictionary<int, ProviderService>();

        public Task<ProviderService> AddAsync(ProviderService item)
        {
            items.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<ProviderService> DeleteAsync(object id)
        {
            ProviderService item = null;

            if (items.TryGetValue((int)id, out item))
            {
                items.Remove((int)id);
                return Task.FromResult(item);
            }
            return Task.FromResult((ProviderService)null);
        }

        public async IAsyncEnumerable<ProviderService> GetAllAsync()
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            ProviderService item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current.Value))
            {
                yield return item;
                await Task.FromResult(enumerator.MoveNext());
            }
        }

        public Task<ProviderService> GetAsync(object id)
        {
            ProviderService item = null;

            items.TryGetValue((int)id, out item);
            return Task.FromResult(item);
        }

        public Task<ProviderService> UpdateAsync(ProviderService changes)
        {
            items[changes.Id] = changes;
            return Task.FromResult(changes);
        }

        public async IAsyncEnumerable<ProviderService> GetAllByNameAsync(string name)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            ProviderService item;

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
