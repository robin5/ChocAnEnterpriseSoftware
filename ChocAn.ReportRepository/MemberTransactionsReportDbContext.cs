// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: MemberTransactionsReportDbContext.cs
// *
// * Description: Defines a DbContext for MemberTransactionsReport entities
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

using Microsoft.EntityFrameworkCore;

namespace ChocAn.ReportRepository
{
    /// <summary>
    /// A ReportDbContext instance represents a session with the database and can be used to
    /// query and save instances of Report entities. 
    /// </summary>
    public class MemberTransactionsReportDbContext : DbContext 
    {
        /// <summary>
        /// Initializes a new instance of the ReportDbContext class
        /// </summary>
        /// <param name="options">The options to be used by the DbContext</param>
        public MemberTransactionsReportDbContext(DbContextOptions<MemberTransactionsReportDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Provides access to Report entities in the database
        /// </summary>
        public DbSet<MemberTransactionsReport> MemberTransactionsReports { get; set; }
    }
}