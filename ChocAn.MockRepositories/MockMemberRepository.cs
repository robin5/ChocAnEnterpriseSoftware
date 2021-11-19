// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MockProviderRepository.cs
// *
// * Description: Mocks the MemberService class
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
using ChocAn.MemberRepository;

namespace ChocAn.MockRepositories
{
    public class MockMemberRepository : IRepository<Member>
    {
        protected IDictionary<int, Member> items = new Dictionary<int, Member>();

        public Task<Member> AddAsync(Member item)
        {
            items.Add(item.Id, item);
            return Task.FromResult(item);
        }

        public Task<Member> DeleteAsync(object id)
        {
            Member item = null;

            if (items.TryGetValue((int)id, out item))
            {
                items.Remove((int)id);
                return Task.FromResult(item);
            }
            return Task.FromResult((Member)null);
        }

        public async IAsyncEnumerable<Member> GetAllAsync()
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Member item;

            enumerator.MoveNext();
            while (null != (item = enumerator.Current.Value))
            {
                yield return item;
                await Task.FromResult(enumerator.MoveNext());
            }
        }

        public Task<Member> GetAsync(object id)
        {
            Member item = null;

            items.TryGetValue((int)id, out item);
            return Task.FromResult(item);
        }

        public Task<Member> UpdateAsync(Member memberChanges)
        {
            items[memberChanges.Id] = memberChanges;
            return Task.FromResult(memberChanges);
        }

        public async IAsyncEnumerable<Member> GetAllByNameAsync(string name)
        {
            var enumerator = items.AsEnumerable().GetEnumerator();
            Member item;

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
