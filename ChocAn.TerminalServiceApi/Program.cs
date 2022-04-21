// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Program.cs
// *
// * Description: Application startup class
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

using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using ChocAn.TerminalServiceApi.Filters;
using ChocAn.Services;
using ChocAn.Services.DefaultMemberService;
using ChocAn.Services.DefaultProviderService;
using ChocAn.Services.DefaultProductService;
using ChocAn.Services.DefaultTransactionService;
using System.Net.Http.Headers;
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProductRepository;
using ChocAn.TransactionRepository;
using ChocAn.MemberServiceApi.Resources;
using ChocAn.ProviderServiceApi.Resources;
using ChocAn.ProductServiceApi.Resources;
using ChocAn.TransactionServiceApi.Resources;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMvc(options =>
{
    options.Filters.Add<RequireHttpsOrCloseAttribute>();
});

// --------------------------------------
// Define dependencies for IMemberService
// --------------------------------------

builder.Services.AddHttpClient<IService<MemberResource, Member>, DefaultMemberService>(
    DefaultMemberService.HttpClientName, client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Services:ChocAn.MemberServiceApi"]);
    }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

builder.Services.AddScoped<IService<MemberResource, Member>, DefaultMemberService>();

// ----------------------------------------
// Define dependencies for IProviderService
// ----------------------------------------

builder.Services.AddHttpClient<IService<ProviderResource, Provider>, DefaultProviderService>(
    DefaultProviderService.HttpClientName, client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Services:ChocAn.ProviderServiceApi"]);
    }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

builder.Services.AddScoped<IService<ProviderResource, Provider>, DefaultProviderService>();

// ---------------------------------------
// Define dependencies for IProductService
// ---------------------------------------

builder.Services.AddHttpClient<IService<ProductResource, Product>, DefaultProductService>(
    DefaultProductService.HttpClientName, client =>
    {
        client.BaseAddress = new Uri(builder.Configuration["Services:ChocAn.ProductServiceApi"]);
    }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

builder.Services.AddScoped<IService<ProductResource, Product>, DefaultProductService>();

// -------------------------------------------
// Define dependencies for ITransactionService
// -------------------------------------------

builder.Services.AddHttpClient<IService<TransactionResource, Transaction>, DefaultTransactionService>(
    DefaultTransactionService.HttpClientName, client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:ChocAn.TransactionServiceApi"]);
    //client.DefaultRequestHeaders
    //  .Accept
    //  .Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header

}).SetHandlerLifetime(TimeSpan.FromMinutes(2));

builder.Services.AddScoped<IService<TransactionResource, Transaction>, DefaultTransactionService>();

// Add API versioning services

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new MediaTypeApiVersionReader();
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
});

//Add Cross site
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowTesting", policy => policy
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

options.AddPolicy("AllowProviderTerminal", policy => policy
.WithOrigins("https://localhost:44380")
.AllowAnyMethod()
.AllowAnyHeader());
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    //app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowTesting");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
