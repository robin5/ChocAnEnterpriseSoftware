// **********************************************************************************
// * Copyright (c) 2021 Robin Murray
// **********************************************************************************
// *
// * File: Startup.cs
// *
// * Description: Configures DataCenterConsole application.
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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ChocAn.Data;
using ChocAn.Services;
using ChocAn.Services.DefaultMemberService;
using ChocAn.Services.DefaultProviderService;
using ChocAn.Services.DefaultProductService;
using ChocAn.DataCenterConsole.Infrastructure;
using ChocAn.DataCenterConsole.Actions;
using ChocAn.DataCenterConsole.Models;
using ChocAn.MemberServiceApi.Resources;
using ChocAn.ProviderServiceApi.Resources;
using ChocAn.ProductServiceApi.Resources;

namespace ChocAn.DataCenterConsole
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
            services.AddControllersWithViews();

            // --------------------------------------
            // Define dependencies for IMemberService
            // --------------------------------------

            services.AddHttpClient<IService<MemberResource, Member>, DefaultMemberService>(
                DefaultMemberService.HttpClientName, client =>
                {
                    client.BaseAddress = new Uri(Configuration["Services:ChocAn.MemberServiceApi"]);
                }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

            services.AddScoped<IService<MemberResource, Member>, DefaultMemberService>();

            // ----------------------------------------
            // Define dependencies for IProviderService
            // ----------------------------------------

            services.AddHttpClient<IService<ProviderResource, Provider>, DefaultProviderService>(
                DefaultProviderService.HttpClientName, client =>
                {
                    client.BaseAddress = new Uri(Configuration["Services:ChocAn.ProviderServiceApi"]);
                }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

            services.AddScoped<IService<ProviderResource, Provider>, DefaultProviderService>();

            // ---------------------------------------
            // Define dependencies for IProductService
            // ---------------------------------------

            services.AddHttpClient<IService<ProductResource, Product>, DefaultProductService>(
                DefaultProductService.HttpClientName, client =>
                {
                    client.BaseAddress = new Uri(Configuration["Services:ChocAn.ProductServiceApi"]);
                }).SetHandlerLifetime(TimeSpan.FromMinutes(2));

            services.AddScoped<IService<ProductResource, Product>, DefaultProductService>();

            // MemberController actions
            services.AddTransient<IIndexAction<MemberResource, Member>, IndexAction<MemberResource, Member, MemberIndexViewModel>>();
            services.AddTransient<IDetailsAction<MemberResource, Member>, DetailsAction<MemberResource, Member, MemberDetailsViewModel>>();
            services.AddTransient<ICreateAction<MemberResource, Member, MemberCreateViewModel>, CreateAction<MemberResource, Member, MemberCreateViewModel>>();
            services.AddTransient<IEditAction<MemberResource, Member, MemberEditViewModel>, EditAction<MemberResource, Member, MemberEditViewModel>>();
            services.AddTransient<IDeleteAction<MemberResource, Member>, DeleteAction<MemberResource, Member>>();

            // ProviderController actions
            services.AddTransient<IIndexAction<ProviderResource, Provider>, IndexAction<ProviderResource, Provider, ProviderIndexViewModel>>();
            services.AddTransient<IDetailsAction<ProviderResource, Provider>, DetailsAction<ProviderResource, Provider, ProviderDetailsViewModel>>();
            services.AddTransient<ICreateAction<ProviderResource, Provider, ProviderCreateViewModel>, CreateAction<ProviderResource, Provider, ProviderCreateViewModel>>();
            services.AddTransient<IEditAction<ProviderResource, Provider, ProviderEditViewModel>, EditAction<ProviderResource, Provider, ProviderEditViewModel>>();
            services.AddTransient<IDeleteAction<ProviderResource, Provider>, DeleteAction<ProviderResource, Provider>>();

            // ProductController actions
            services.AddTransient<IIndexAction<ProductResource, Product>, IndexAction<ProductResource, Product, ProductIndexViewModel>>();
            services.AddTransient<IDetailsAction<ProductResource, Product>, DetailsAction<ProductResource, Product, ProductDetailsViewModel>>();
            services.AddTransient<ICreateAction<ProductResource, Product, ProductCreateViewModel>, CreateAction<ProductResource, Product, ProductCreateViewModel>>();
            services.AddTransient<IEditAction<ProductResource, Product, ProductEditViewModel>, EditAction<ProductResource, Product, ProductEditViewModel>>();
            services.AddTransient<IDeleteAction<ProductResource, Product>, DeleteAction<ProductResource, Product>>();

            services.AddAutoMapper(options => options.AddProfile<MappingProfile>());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
