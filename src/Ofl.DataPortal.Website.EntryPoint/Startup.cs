using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;

namespace Ofl.DataPortal.Website.EntryPoint
{
    public class Startup
    {
        public Startup(
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment
        )
        {
            // Assign values.
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        private readonly IConfiguration _configuration;

        private readonly IWebHostEnvironment _webHostEnvironment; 

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // For ease of use.
            var sc = services;

            // Authentication and authorization
            sc.AddAuthentication();
            sc = sc.AddAuthorization();

            // Add configuration.
            sc = sc.AddSingleton(_configuration);

            // Add options.
            sc = sc.AddOptions();

            // Add MVC.
            sc.AddOflMvc();

            // Add http context accessor.
            sc = sc.AddHttpContextAccessor();

            // In production, the React files will be served from this directory
            sc.AddSpaStaticFiles(configuration => {
                // Add built files.
                configuration.RootPath = "ClientApp/build";
            });
        }

        private const string GraphQLEndpoint = "/api/graphql";

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IWebHostEnvironment env,
            IServiceProvider serviceProvider
        )
        {
            // Forward HTTPS from the container.
            app = app.ForwardHttpsFromContainer();

            // Developer exception page in all environments.
            app = app.UseDeveloperExceptionPage();

            // If not developing, set up prod environment.
            if (!env.IsDevelopment())
            {
                // Use the error handler.
                // TODO: Get old behavior, see that it works with create-react-app
                //app = app.UseExceptionHandler("/Error");
                app = app.UseHsts();
            }

            // Redirect HTTPS.
            app = app.UseHttpsRedirection();

            // Use static files.
            app = app.UseStaticFiles();

            // Set up static files for SPA.
            app.UseSpaStaticFiles();

            // Done *after* static files to omit it from being logged, as per:
            // https://nblumhardt.com/2019/10/serilog-in-aspnetcore-3/
            // Section: Streamlined request logging
            app = app.UseSerilogRequestLogging();

            // Setup routing.
            app = app.UseRouting();

            // Authentication and authorization.
            app = app.UseAuthentication();
            app = app.UseAuthorization();

            // Set up endpoints.
            app = app.UseEndpoints(b => {
                // Map controllers.
                b.MapControllers();
            });

            // Set up the single page application for react.
            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
