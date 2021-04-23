using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Quartz;
using Serilog;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AX.QuartzServer.Core
{
    public static class AXQuartzConfigExtensions
    {
        public static void InitJobAndTriggerFromJobsettings(this IServiceCollectionQuartzConfigurator quartz, IConfiguration configuration)
        {
            var allJobs = configuration.GetSection("Jobs").Get<List<BaseJobConfig>>();

            Log.Logger.Information($"开始注册 Job");
            Log.Logger.Information($"共获取到 {allJobs.Count} 个 Job");

            foreach (var item in allJobs)
            {
                Log.Logger.Information($"{JsonConvert.SerializeObject(item)}");

                var jobName = $"{item.JobType}_{item.Name}";
                var jobKey = new JobKey(jobName);
                Log.Logger.Information($"{nameof(jobKey)}_{jobKey}");

                var jobData = new JobDataMap();
                jobData.PutAll(ToIDictionary(item));

                if (item.JobType.ToLower().Contains("testjob"))
                { quartz.AddJob<Jobs.TestJob>(opts => { opts.WithIdentity(jobKey); opts.SetJobData(jobData); }); }
                if (item.JobType.ToLower().Contains("windowscmdjob"))
                { quartz.AddJob<Jobs.WindowsCMDJob>(opts => { opts.WithIdentity(jobKey); opts.SetJobData(jobData); }); }

                quartz.AddTrigger(opts => opts
                    .ForJob(jobKey)
                    .WithIdentity($"{jobName}_Trigger")
                    .WithCronSchedule(item.Cron));
            }

            Log.Logger.Information($"结束注册 Job");
        }

        private static IDictionary<string, object> ToIDictionary<T>(T value)
        {
            IDictionary<string, object> result = new Dictionary<string, object>();
            Type type = typeof(T);
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo p in propertyInfos)
            {
                result.Add(p.Name, p.GetValue(value, null));
            }
            return result;
        }
    }

    /// <summary>
    /// 配置类
    /// </summary>
    internal class BaseJobConfig
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 任务类型
        /// </summary>
        public string JobType { get; set; }

        /// <summary>
        /// Cron 执行时间公式
        /// </summary>
        public string Cron { get; set; }
    }

    internal class WindowsCMDJobConfig : BaseJobConfig
    {
        /// <summary>
        /// 执行程序名
        /// </summary>
        public string CMDFileName { get; set; }

        /// <summary>
        /// 执行程序参数
        /// </summary>
        public string CMDParm { get; set; }
    }
}