using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SF.Core.Logging;
using SF.Core.ServiceManagement;

namespace SF.Core.CallPlans.Runtime
{
	public static class CallScheduler
	{
        static ILogger GetLogger(IServiceProvider sp)
        {
            return sp.Resolve<ILogService>().GetLogger("可靠调用运行器");
        }
        public static async Task Startup(IServiceProvider sp)
        {
            var Logger = GetLogger(sp);
            var Dispatcher = sp.Resolve<ICallDispatcher>();
            Logger.Info("执行可靠调用清理过程");
            try
            {
                var re = await Dispatcher.SystemStartupCleanup();
                Logger.Info("清理{0}个未终止可靠调用", re);
            }
            catch (Exception e)
            {
                Logger.Error(e, "执行清理过程时发生异常");
            }
        }
        public static async Task Execute(IServiceProvider sp,int count=10)
        {
            var Logger = GetLogger(sp);
            var Dispatcher = sp.Resolve<ICallDispatcher>();
            try
            {
                await Dispatcher.Execute(count);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "未捕获异常");
            }
        }
	}
}
