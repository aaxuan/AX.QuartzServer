using AX.QuartzServer.Core;
using Microsoft.Extensions.Hosting;
using Quartz;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace AX.QuartzServer.WorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", @"all-log.log");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.File(logpath, rollingInterval: RollingInterval.Day)
                //.WriteTo.Console()
                //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(Path.Combine("logs", @"Information-log.log"), rollingInterval: RollingInterval.Day))
                //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(Path.Combine("logs", @"Debug-log.log"), rollingInterval: RollingInterval.Day))
                //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(Path.Combine("logs", @"Warning-log.log"), rollingInterval: RollingInterval.Day))
                //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(Path.Combine("logs", @"Error-log.log"), rollingInterval: RollingInterval.Day))
                //.WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(Path.Combine("logs", @"Fatal-log.log"), rollingInterval: RollingInterval.Day))
                .CreateLogger();

            // C:\WINDOWS\system32
            Log.Logger.Information($"Environment.CurrentDirectory : {Environment.CurrentDirectory}");
            // exe ËùÔÚÂ·¾¶
            Log.Logger.Information($"AppDomain.CurrentDomain.BaseDirectory:{AppDomain.CurrentDomain.BaseDirectory}");

            Log.Logger.Information(logpath);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddQuartz(q =>
                {
                    q.UseMicrosoftDependencyInjectionScopedJobFactory();
                    q.InitJobAndTriggerFromJobsettings(hostContext.Configuration);
                });

                services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
                //services.AddHostedService<Worker>();
            });

            //Windows
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            { hostBuilder.UseWindowsService(); }

            //Linux
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            { hostBuilder.UseSystemd(); }

            return hostBuilder;
        }
    }
}