// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MembersController.cs
// *
// * Description: Implements the Member controller for the MemberService API.
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ChocAn.Data;
using ChocAn.Repository;
using ChocAn.Repository.Paging;
using ChocAn.Repository.Sorting;
using ChocAn.Repository.Search;

namespace ChocAn.MemberServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> logger;
        private readonly IRepository<Member> repository;
        private readonly PagingOptions defaultPagingOptions;
        public MemberController(
            ILogger<MemberController> logger,
            IRepository<Member> repository,
            IOptions<PagingOptions> defaultPagingOptions)
        {
            this.logger = logger;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
        }

        /// <summary>
        /// Retrieves all members from Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<Member> sortOptions,
            [FromQuery] SearchOptions<Member> searchOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<Member> members = new();
                await foreach (Member member in repository.GetAllAsync(pagingOptions, sortOptions, searchOptions))
                {
                    members.Add(member);
                }
                return Ok(members);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Retrieves an individual member from the Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 404 if member does not exist. 500 on exception</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var member = await repository.GetAsync(id);
                if (null == member)
                {
                    return NotFound();
                }
                return Ok(member);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new member into the Member repository.
        /// </summary>
        /// <param name="member"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] Member member)
        {
            try
            {
                // Verify member's ID = 0 to enforce good behavior
                if (member.Id != 0)
                    return BadRequest();

                var result = await repository.AddAsync(new Member()
                {
                    Name = member.Name,
                    Email = member.Email,
                    StreetAddress = member.StreetAddress,
                    City = member.City,
                    State = member.State,
                    ZipCode = member.ZipCode,
                    Status = member.Status
                });

                if (null == result)
                    return BadRequest();

                return Created("", result);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Updates a member in the Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <param name="member">Member updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] Member member)
        {
            try
            {
                // Verify member's ID and the ID of the endpoint are the same
                if (member.Id != id)
                    return BadRequest();

                var numChanged = await repository.UpdateAsync(new Member
                {
                    Id = member.Id,
                    Name = member.Name,
                    Email = member.Email,
                    StreetAddress = member.StreetAddress,
                    City = member.City,
                    State = member.State,
                    ZipCode = member.ZipCode,
                    Status = member.Status
                });

                if (numChanged > 0)
                    return Ok();
                else
                    return NotFound();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger?.LogError(ex, nameof(PutAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(PutAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Deletes a member from the Member respoitory.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 404 if member does not exist. 500 on exception</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var member = await repository.DeleteAsync(id);
                if (null == member)
                {
                    return NotFound();
                }
                return Ok(member);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
