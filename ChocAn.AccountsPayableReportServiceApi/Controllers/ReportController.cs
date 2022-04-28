// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: ReportController.cs
// *
// * Description: Implements the Report controller for the AccountsPayableReportServiceApi.
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
using ChocAn.RabbitMQMessages;

namespace ChocAn.AccountsPayableReportServiceApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> logger;
        private readonly IRepository<AccountsPayableReport> repository;
        private readonly PagingOptions defaultPagingOptions;
        private readonly IRabbitMQMessage<AccountsPayableReport> reportMessage;
        public ReportController(
            ILogger<ReportController> logger,
            IRepository<AccountsPayableReport> repository,
            IOptions<PagingOptions> defaultPagingOptions,
            IRabbitMQMessage<AccountsPayableReport> reportMessage)
        {
            this.logger = logger;
            this.repository = repository;
            this.defaultPagingOptions = defaultPagingOptions.Value;
            this.reportMessage = reportMessage;
        }

        /// <summary>
        /// Retrieves all reports from Report repository.
        /// </summary>
        /// <param name="id">Report's identification number</param>
        /// <returns>200 on success. 500 on exception</returns>
        [HttpGet()]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetAllAsync(
            [FromQuery] PagingOptions pagingOptions,
            [FromQuery] SortOptions<AccountsPayableReport> sortOptions,
            [FromQuery] SearchOptions<AccountsPayableReport> searchOptions)
        {
            try
            {
                pagingOptions.Offset ??= defaultPagingOptions.Offset;
                pagingOptions.Limit ??= defaultPagingOptions.Limit;

                List<AccountsPayableReport> reports = new();

                await foreach (AccountsPayableReport report in repository.GetAllAsync(pagingOptions, sortOptions, searchOptions))
                {
                    reports.Add(report);
                }
                return Ok(reports);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAllAsync));
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
                var report = await repository.GetAsync(id);
                if (null == report)
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(GetAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Inserts a new report into the Report repository.
        /// </summary>
        /// <param name="report"></param>
        /// <returns>201 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPost()]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] AccountsPayableReport report)
        {
            try
            {
                // Verify report's ID = 0 to enforce good behavior
                if (report.Id != 0)
                    return BadRequest();

                var result = await repository.AddAsync(new AccountsPayableReport
                {
                    OwnerId = report.OwnerId,
                    Name = report.Name,
                    StartDate = report.StartDate,
                    EndDate = report.EndDate,
                    Created = DateTime.Now,
                    DocumentId = null,
                });

                if (null == result)
                    return BadRequest();

                // Send message to start report generation
                reportMessage.Send("AccountsPayableReport-Create", result);

                return Created("", result);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(PostAsync));
                return Problem();
            }
        }

        /// <summary>
        /// Updates a report in the AccountsPayableReportRepository.
        /// </summary>
        /// <param name="id">AccountsPayableReport's identification number</param>
        /// <param name="report">AccountsPayableReport updates</param>
        /// <returns>200 on success. 400 on validation errors. 500 on exception</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PutAsync(int id, [FromBody] AccountsPayableReport report)
        {
            try
            {
                // Verify member's ID and the ID of the endpoint are the same
                if (report.Id != id)
                    return BadRequest();

                var numChanged = await repository.UpdateAsync(new AccountsPayableReport
                {
                    Id = report.Id,
                    OwnerId = report.OwnerId,
                    Name = report.Name,
                    StartDate = report.StartDate,
                    EndDate = report.EndDate,
                    Created = DateTime.Now,
                    DocumentId = null,
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
                var report = await repository.DeleteAsync(id);
                if (null == report)
                {
                    return NotFound();
                }
                return Ok(report);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, nameof(DeleteAsync));
                return Problem();
            }
        }
    }
}
