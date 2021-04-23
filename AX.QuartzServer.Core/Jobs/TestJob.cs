using Quartz;
using Serilog;
using System;
using System.Threading.Tasks;

namespace AX.QuartzServer.Core.Jobs
{
    [DisallowConcurrentExecution]
    public class TestJob : AXQuartzJob
    {
        public string Name { get { return "测试Job"; } }
        public string Note { get { return "会打印日志"; } }

        public Task Execute(IJobExecutionContext context)
        {
            Log.Logger.Information($"{Newtonsoft.Json.JsonConvert.SerializeObject(context.JobDetail.JobDataMap)}");
            Log.Logger.Information($"Hello world! {DateTime.Now.ToLongTimeString()}");
            return Task.CompletedTask;
        }
    }
}