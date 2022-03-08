// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Startup.cs
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

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProductRepository;
using ChocAn.TransactionRepository;
using Microsoft.EntityFrameworkCore;
using ChocAn.ProviderTerminal.Api.Filters;
using ChocAn.Repository;
using ChocAn.Services;
using ChocAn.Services.DefaultMemberService;
using ChocAn.Services.DefaultProviderService;
using ChocAn.Services.DefaultProductService;
using ChocAn.Services.DefaultTransactionService;

namespace ChocAn.ProviderTerminal.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
            {
                options.Filters.Add<RequireHttpsOrCloseAttribute>();
            });

            string tmp = Configuration["Services:ChocAn.MemberServiceApi"];
            tmp = Configuration["Services:ChocAn.ProviderServiceApi"];

            // Define dependencies for IMemberService
            services.AddHttpClient<IMemberService, DefaultMemberService>(DefaultMemberService.Name, client =>
            {
                client.BaseAddress = new Uri(Configuration["Services:ChocAn.MemberServiceApi"]);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(2));
            services.AddScoped<IMemberService, DefaultMemberService>();

            // Define dependencies for IProviderService
            services.AddHttpClient<IProviderService, DefaultProviderService>(DefaultProviderService.Name, client =>
            {
                client.BaseAddress = new Uri(Configuration["Services:ChocAn.ProviderServiceApi"]);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(2));
            services.AddScoped<IProviderService, DefaultProviderService>();

            // Define dependencies for IProductService
            services.AddHttpClient<IProductService, DefaultProductService>(DefaultProductService.Name, client =>
            {
                client.BaseAddress = new Uri(Configuration["Services:ChocAn.ProductServiceApi"]);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(2));
            services.AddScoped<IProductService, DefaultProductService>();

            // Define dependencies for ITransactionService
            services.AddHttpClient<ITransactionService, DefaultTransactionService>(DefaultTransactionService.Name, client =>
            {
                client.BaseAddress = new Uri(Configuration["Services:ChocAn.TransactionServiceApi"]);
            }).SetHandlerLifetime(TimeSpan.FromMinutes(2));
            services.AddScoped<ITransactionService, DefaultTransactionService>();

            // Add API versioning services
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.ApiVersionReader = new MediaTypeApiVersionReader();
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options);
            });

            // Add Cross site
            services.AddCors(options =>
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChocAn.ProviderTerminal.API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChocAn.ProviderTerminal.API v1"));
                app.UseCors("AllowTesting");
            }
            else
            {
                app.UseHsts();
            }

            // app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
