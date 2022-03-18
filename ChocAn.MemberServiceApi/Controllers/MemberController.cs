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

using ChocAn.Repository;
using Microsoft.AspNetCore.Mvc;
using ChocAn.MemberRepository;
using ChocAn.MemberServiceApi.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace ChocAn.MemberServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly ILogger<MemberController> logger;
        private readonly IMapper mapper;
        private readonly IRepository<MemberRepository.Member> memberRepository;
        public MemberController(
            ILogger<MemberController> logger,
            IMapper mapper,
            IRepository<MemberRepository.Member> memberRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.memberRepository = memberRepository;
        }

        /// <summary>
        /// Retrieves all members from Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet(Name = nameof(GetAllAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<Member> members = new();
                await foreach (Member member in memberRepository.GetAllAsync())
                {
                    members.Add(member);
                }

                return Ok(members);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllAsync));
                return Problem();
            }
        }
        /// <summary>
        /// Retrieves an individual member from the Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 404 if member does not exist. 500 on exception</returns>
        [HttpGet("{id}", Name = nameof(GetAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var member = await memberRepository.GetAsync(id);
                if (null == member)
                {
                    return NotFound();
                }

                return Ok(member);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new member into the Member repository.
        /// </summary>
        /// <param name="memberResource"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost(Name = nameof(PostAsync))]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] MemberResource memberResource)
        {
            try
            {
                var member = mapper.Map<Member>(memberResource);
                await memberRepository.AddAsync(member);
                return Created("", memberResource);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }


        /// <summary>
        /// Updates a member in the Member repository.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <param name="memberResource">Member updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}", Name = nameof(PutAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] MemberResource memberResource)
        {
            try
            {
                var member = mapper.Map<Member>(memberResource);
                member.Id = id;
                await memberRepository.UpdateAsync(member);

                return Ok(memberResource);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, nameof(PutAsync));
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PutAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Deletes a member from the Member respoitory.
        /// </summary>
        /// <param name="id">Member's identification number</param>
        /// <returns>200 on success. 404 if member does not exist. 500 on exception</returns>
        [HttpDelete("{id}", Name = nameof(DeleteAsync))]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var member = await memberRepository.DeleteAsync(id);
                if (null == member)
                {
                    return NotFound();
                }
                return Ok(member);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
