// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ProviderTransactionsReportController.cs
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
using ChocAn.ReportRepository;
using ChocAn.ReportService.Resources;
using Microsoft.EntityFrameworkCore;
using ChocAn.Repository.Paging;

namespace ChocAn.ReportService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProviderTransactionsReportController : ControllerBase
    {
        public const string GetAllAsyncExceptionMessage = "Exception while processing request for api/ProviderTransactionsReport/GetAllAsync";
        public const string GetAsyncExceptionMessage = "Exception while processing request for api/ProviderTransactionsReport/GetAllAsync/{id}";
        public const string PostAsyncExceptionMessage = "Exception while processing request for api/ProviderTransactionsReport/PostAsync";
        public const string PutAsyncExceptionMessage = "Exception while processing request for api/ProviderTransactionsReport/PutAsync";
        public const string DeleteAsyncExceptionMessage = "Exception while processing request for api/ProviderTransactionsReport/DeleteAsync";

        private readonly ILogger<ProviderTransactionsReportController> logger;
        private readonly IProviderTransactionsReportRepository reportRepository;

        public ProviderTransactionsReportController(
            ILogger<ProviderTransactionsReportController> logger,
            IProviderTransactionsReportRepository reportRepository)
        {
            this.logger = logger;
            this.reportRepository = reportRepository;
        }

        /// <summary>
        /// Retrieves all reports from Report repository.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync([FromQuery] PagingOptions pagingOptions)
        {
            try
            {
                List<ProviderTransactionsReport> reports = new();
                await foreach (ProviderTransactionsReport report in reportRepository.GetAllAsync(pagingOptions))
                {
                    reports.Add(report);
                }

                return Ok(reports);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, GetAllAsyncExceptionMessage);
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
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, GetAsyncExceptionMessage, id);
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
        public async Task<IActionResult> PostAsync([FromBody] ProviderTransactionsReportResource resource)
        {
            try
            {
                var report = new ProviderTransactionsReport()
                {
                    Id = 0,
                    Name = resource.Name,
                    OwnerId = resource.OwnerId,
                    StartDate = resource.StartDate,
                    EndDate = resource.EndDate,
                    Status = resource.Status,
                    Created = resource.Created,
                    ProviderId = resource.ProviderId
                };
                await reportRepository.AddAsync(report);
                return Created("", resource);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, PostAsyncExceptionMessage);
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
        public async Task<IActionResult> PutAsync(int id, [FromBody] ProviderTransactionsReportResource reportResource)
        {
            try
            {
                var report = new ProviderTransactionsReport()
                {
                    Id = id,
                    Name = reportResource.Name,
                    OwnerId = reportResource.OwnerId,
                    StartDate = reportResource.StartDate,
                    EndDate = reportResource.EndDate,
                    Status = reportResource.Status,
                    Created = reportResource.Created
                };
                await reportRepository.UpdateAsync(report);
                return Ok(reportResource);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                logger?.LogError(ex, PutAsyncExceptionMessage);
                return BadRequest();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, PutAsyncExceptionMessage);
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
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, DeleteAsyncExceptionMessage);
                return Problem();
            }
        }
    }
}
