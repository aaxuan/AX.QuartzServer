using Microsoft.Extensions.Hosting;
using Serilog;
using System.Threading;
using System.Threading.Tasks;

namespace AX.QuartzServer.WorkerService
{
    public class Worker : BackgroundService
    {
        public Worker()
        { }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Log.Logger.Information("ExecuteAsync Begin");
            Log.Logger.Information(Newtonsoft.Json.JsonConvert.SerializeObject(stoppingToken));

            //await scheduler.Start(stoppingToken);
            //把Job和触发器放入调度器中
            //await scheduler.ScheduleJob(sendMsgJob, sendMsgTrigger, stoppingToken);
        }
    }
}