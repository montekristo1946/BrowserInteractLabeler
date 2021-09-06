using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.RollingFileSizeLimit.Extensions;
using Serilog.Sinks.RollingFileSizeLimit.Impl;

namespace BrowserInteractLabeler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            var logger = Log.ForContext<Program>();
            logger.Debug("Start Application");
          
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog((ctx, logCfg) =>
                {
                    var logDirectoryPath = "/var/log";
                    var outputTemplate = "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] [{Level:u3}] {Message:lj}{NewLine}{Exception}";
                    logCfg
                        .Enrich.WithProcessName()
                        .Enrich.WithProcessId()
                        .Enrich.WithExceptionDetails()
                        .Enrich.WithMachineName()
                        .Enrich.WithEnvironmentUserName()
                        .MinimumLevel.Is(LogEventLevel.Debug)
                        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                        .WriteTo.RollingFileSizeLimited(logDirectoryPath,
                            Path.Combine(logDirectoryPath, "Archive"),
                            fileSizeLimitBytes: 50 * 1024 * 1024,
                            archiveSizeLimitBytes: 10 * 50 * 1024 * 1024,
                            logFilePrefix: nameof(BrowserInteractLabeler),
                            fileCompressor: new DefaultFileCompressor(),
                            outputTemplate: outputTemplate)
                        .WriteTo.Console();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls("http://localhost:5000");
                });
    }
}