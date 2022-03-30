// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Program.cs
// *
// * Description: Application startup class for ChocAn.ReportServiceApi project.
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

using ChocAn.ReportRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Add services to the container
// -----------------------------

builder.Services.AddDbContextPool<MemberTransactionsReportDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("MemberTransactionsReportConnection")));
builder.Services.AddScoped<IReportRepository<MemberTransactionsReport>, DefaultMemberTransactionsReportRepository>();

builder.Services.AddDbContextPool<ProviderTransactionsReportDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("ProviderTransactionsReportConnection")));
builder.Services.AddScoped<IReportRepository<ProviderTransactionsReport>, DefaultProviderTransactionsReportRepository>();

builder.Services.AddDbContextPool<AccountsPayableReportDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("AccountsPayableReportConnection")));
builder.Services.AddScoped<IReportRepository<AccountsPayableReport>, DefaultAccountsPayableReportRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
// -----------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
