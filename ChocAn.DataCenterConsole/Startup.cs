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

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using ChocAn.MemberRepository;
using ChocAn.ProviderRepository;
using ChocAn.ProductRepository;
using ChocAn.DataCenterConsole.Infrastructure;
using ChocAn.DataCenterConsole.Actions;
using ChocAn.DataCenterConsole.Models;
using ChocAn.Repository;

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
            services.AddDbContextPool<MemberDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("MemberDbConnection")));
            
            services.AddDbContextPool<ProviderDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("MemberDbConnection")));
            
            services.AddDbContextPool<ProductDbContext>(options => options.UseSqlServer(
                Configuration.GetConnectionString("MemberDbConnection")));

            services.AddControllersWithViews();

            // MemberController actions
            services.AddTransient<IIndexAction<Member>, IndexAction<Member, MemberIndexViewModel>>();
            services.AddTransient<IDetailsAction<Member>, DetailsAction<Member, MemberDetailsViewModel>>();
            services.AddTransient<ICreateAction<Member, MemberCreateViewModel>, CreateAction<Member, MemberCreateViewModel>>();
            services.AddTransient<IEditAction<Member, MemberEditViewModel>, EditAction<Member, MemberEditViewModel>>();
            services.AddTransient<IDeleteAction<Member>, DeleteAction<Member>>();

            // ProviderController actions
            services.AddTransient<IIndexAction<Provider>, IndexAction<Provider, ProviderIndexViewModel>>();
            services.AddTransient<IDetailsAction<Provider>, DetailsAction<Provider, ProviderDetailsViewModel>>();
            services.AddTransient<ICreateAction<Provider, ProviderCreateViewModel>, CreateAction<Provider, ProviderCreateViewModel>>();
            services.AddTransient<IEditAction<Provider, ProviderEditViewModel>, EditAction<Provider, ProviderEditViewModel>>();
            services.AddTransient<IDeleteAction<Provider>, DeleteAction<Provider>>();

            // ProductController actions
            services.AddTransient<IIndexAction<Product>, IndexAction<Product, ProductIndexViewModel>>();
            services.AddTransient<IDetailsAction<Product>, DetailsAction<Product, ProductDetailsViewModel>>();
            services.AddTransient<ICreateAction<Product, ProductCreateViewModel>, CreateAction<Product, ProductCreateViewModel>>();
            services.AddTransient<IEditAction<Product, ProductEditViewModel>, EditAction<Product, ProductEditViewModel>>();
            services.AddTransient<IDeleteAction<Product>, DeleteAction<Product>>();

            // Repositories
            services.AddScoped<IRepository<Member>, DefaultMemberRepository>();
            services.AddScoped<IRepository<Provider>, DefaultProviderRepository>();
            services.AddScoped<IRepository<Product>, DefaultProductRepository>();
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
