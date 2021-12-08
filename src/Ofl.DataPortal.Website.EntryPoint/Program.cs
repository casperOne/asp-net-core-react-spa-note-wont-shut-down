using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Ofl.DataPortal.Website.EntryPoint
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Set up logging explicitly to the console for setup of the app
            // Logging configuration will take over when app starts.
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            try
            {
                Log.Information($"Starting {typeof(Program).Assembly.FullName}...");

                // Run.
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                // Log the error.
                Log.Fatal(ex, $"Startup of {typeof(Program).Assembly.FullName} failed.");
            }
            finally
            {
                // Flush.
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                // Validate scopes.
                .UseDefaultServiceProvider((c, o) => o.ValidateScopes = true)
                .UseSerilog((context, config) => config
                    .Enrich.FromLogContext()
                    .MinimumLevel.Verbose()
                    .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] - {Message}{NewLine}{Exception}")
                    .WriteTo.Debug(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] [{SourceContext}] - {Message}{NewLine}{Exception}")
                )
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
