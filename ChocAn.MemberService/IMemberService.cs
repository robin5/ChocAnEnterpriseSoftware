// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: IMemberService.cs
// *
// * Description: IMemberService defines an interface for storing Member objects
// *              in a database
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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChocAn.MemberService
{
    /// <summary>
    /// Represents repository pattern for Member entities
    /// </summary>
    public interface IMemberService
    {
        /// <summary>
        /// Adds a Member entity to the database
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        Task<Member> AddAsync(Member member);

        /// <summary>
        /// Retrieves a Member entity from the database
        /// </summary>
        /// <param name="id">ID of Member entity to retrieve</param>
        /// <returns></returns>
        Task<Member> GetMemberAsync(Guid id);

        /// <summary>
        /// Retrieves a Member entity from the database by member number
        /// </summary>
        /// <param name="id">ID of Member entity to retrieve</param>
        /// <returns></returns>
        Task<Member> GetMemberByNumberAsync(decimal number);

        /// <summary>
        /// Updates a Member entity in the database
        /// </summary>
        /// <param name="memberChanges">Changes to be applied to Member entity</param>
        /// <returns></returns>
        Task<Member> UpdateAsync(Member memberChanges);

        /// <summary>
        /// Deletes Member entity from the database
        /// </summary>
        /// <param name="id">ID of member to deleted</param>
        /// <returns></returns>
        Task<Member> DeleteAsync(Guid id);

        /// <summary>
        /// Retrieves all Member entities in the database
        /// </summary>
        /// <returns>An enumerator that provides asynchronous iteration over all Member Entities in the database</returns>
        IAsyncEnumerable<Member> GetAllMembersAsync();
    }
}
