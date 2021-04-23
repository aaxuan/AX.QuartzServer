using Quartz;

namespace AX.QuartzServer.Core
{
    public interface AXQuartzJob : IJob
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Note { get; }
    }
}