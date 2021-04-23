using Quartz;
using Serilog;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AX.QuartzServer.Core.Jobs
{
    public class WindowsCMDJob : AXQuartzJob
    {
        public string Name { get { return "WindowsCMDJob"; } }

        public string Note { get { return "执行 Windows Job 并获得结果"; } }

        public Task Execute(IJobExecutionContext context)
        {
            var data = context.JobDetail.JobDataMap;
            var result = string.Empty; 

            using (var process = new Process())
            {
                var cmdExePath = data.GetString("CmdFileName");
                var cmdParm = data.GetString("CmdParm");

                Log.Logger.Information($"{nameof(cmdExePath)} = {cmdExePath}");
                Log.Logger.Information($"{nameof(cmdParm)} = {cmdParm}");

                process.StartInfo.FileName = $"{cmdExePath}";
                process.StartInfo.Arguments = cmdParm;
                process.StartInfo.UseShellExecute = false;        //是否使用操作系统shell启动
                process.StartInfo.RedirectStandardInput = true;   //接受来自调用程序的输入信息
                process.StartInfo.RedirectStandardOutput = true;  //由调用程序获取输出信息
                process.StartInfo.RedirectStandardError = true;   //重定向标准错误输出
                process.StartInfo.CreateNoWindow = false;          //不显示程序窗口
                process.Start();

                process.StandardInput.WriteLine("");
                process.StandardInput.AutoFlush = true;

                result = process.StandardOutput.ReadToEnd();
                Log.Logger.Information(result);

                process.WaitForExit();//等待程序执行完退出进程
                process.Close();
            }

            Log.Logger.Information("process.Close");
            return Task.CompletedTask;
        }
    }
}