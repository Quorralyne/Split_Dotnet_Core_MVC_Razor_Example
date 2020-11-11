using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Splitio.Services.Client.Classes;

namespace SplitDotnetCoreMVCRazorExample
{
    public class Startup
    {
        public static bool V2NavigationIsActive = false;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();

            var config = new ConfigurationOptions();
            var factory = new SplitFactory("your Split API KEY goes here", config);
            var splitClient = factory.Client();
            try
            {
                splitClient.BlockUntilReady(10000);
            }
            catch (Exception ex)
            {
                // log & handle
            }

            var treatment = splitClient.GetTreatment(Configuration.GetValue<string>("SplitUser"), "v2_navigation_view");
            if (treatment == "on")
            {
                V2NavigationIsActive = true;
            }
            else if (treatment == "off")
            {
                V2NavigationIsActive = false;
            }
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
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}
