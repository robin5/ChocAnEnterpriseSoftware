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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocAn.MemberRepository;

namespace ChocAn.MockRepositories
{
    public class MockMemberRepository : IMemberRepository
    {
        private List<Member> members = new List<Member>();
        public Task<Member> AddAsync(Member member)
        {
            members.Add(member);
            return Task.FromResult(member);
        }

        public Task<Member> DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<Member> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Member> GetByNumberAsync(decimal number)
        {
            return Task.FromResult(members.Find(s => s.Number == number));
        }

        public Task<Member> UpdateAsync(Member memberChanges)
        {
            throw new NotImplementedException();
        }
    }
}
