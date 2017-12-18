#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using SF.Sys.Logging;
using SF.Sys.Services;

namespace SF.Sys.Plans.Runtime
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
