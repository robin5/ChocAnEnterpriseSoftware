﻿// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MemberTransactionsReportController.cs
// *
// * Description: Implements the Report controller for the ReportService API.
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChocAn.ReportRepository;
using ChocAn.TransactionRepository;
using Microsoft.Extensions.Logging;
using ChocAn.ReportService.Resources;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository;

namespace ChocAn.ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberTransactionsReportController : ControllerBase
    {
        private readonly ILogger<MemberTransactionsReportController> logger;
        private readonly IMapper mapper;
        private readonly IReportRepository<MemberTransactionsReport> reportRepository;
        private readonly ITransactionRepository transactionRepository;
        public MemberTransactionsReportController(
            ILogger<MemberTransactionsReportController> logger,
            IMapper mapper,
            IReportRepository<MemberTransactionsReport> reportRepository,
            ITransactionRepository transactionRepository)
        {
            this.logger = logger;
            this.mapper = mapper;
            this.reportRepository = reportRepository;
            this.transactionRepository = transactionRepository;
        }

        /// <summary>
        /// Retrieves all reports from Report repository.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync()
        {
            try
            {
                List<Report> reports = new List<Report>();
                await foreach (Report report in reportRepository.GetAllAsync())
                {
                    reports.Add(report);
                }

                return Ok(reports);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAllAsync), null);
                return Problem();
            }
        }

        /// <summary>
        /// Retrieves an individual report from the Report repository.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <returns>200 on success. 404 if report does not exist. 500 on exception</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAsync(int id)
        {
            try
            {
                var report = await reportRepository.GetAsync(id);
                if (null == report)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<MemberTransactionsReportResource>(report));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(GetAsync), null);
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new report into the Report repository.
        /// </summary>
        /// <param name="reportResource"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] MemberTransactionsReportResource reportResource)
        {
            try
            {
                var report = mapper.Map<MemberTransactionsReport>(reportResource);
                await reportRepository.AddAsync(report);
                return Created("", reportResource);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PostAsync), null);
                return Problem();
            }
        }

        /// <summary>
        /// Updates a report in the Report repository.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <param name="reportResource">Report updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] MemberTransactionsReportResource reportResource)
        {
            try
            {
                var report = mapper.Map<MemberTransactionsReport>(reportResource);
                report.Id = id;
                await reportRepository.UpdateAsync(report);

                return Ok(reportResource);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger.LogError(ex, nameof(PutAsync), null);
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(PutAsync), null);
                return Problem();
            }
        }

        /// <summary>
        /// Deletes a report from the Report respoitory.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <returns>200 on success. 404 if report does not exist. 500 on exception</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteAsync(int id)
        {
            try
            {
                var report = await reportRepository.DeleteAsync(id);
                if (null == report)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<MemberTransactionsReportResource>(report));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(DeleteAsync), null);
                return Problem();
            }
        }
    }
}
