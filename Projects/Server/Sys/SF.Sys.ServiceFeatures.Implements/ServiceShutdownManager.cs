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
using System.Text;
using System.Threading.Tasks;
using SF.Sys.Linq;
using SF.Sys.Logging;
using SF.Sys.Reflection;

namespace SF.Sys.ServiceFeatures
{
    public class ServiceShutdownManager : IServiceShutdownManager
    {
        List<Func<Task>> Callbacks { get; } = new List<Func<Task>>();
        ILogger<ServiceShutdownManager> Logger { get; }
        public bool Shutdowned { get; private set; }
        public ServiceShutdownManager(ILogger<ServiceShutdownManager> Logger)
        {
            this.Logger = Logger;
        }
        void CheckShutdown()
        {
            if (Shutdowned)
                throw new InvalidOperationException();

        }
        public async Task Shutdown()
        {
            CheckShutdown();
            Shutdowned = true;
            Callbacks.Reverse();
            foreach (var cb in Callbacks)
            {
                try
                {
                    await cb();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "AppSutdownError");
                }
            }
        }

        public void Register(Func<Task> Callback)
        {
            CheckShutdown();
            Callbacks.Add(Callback);
        }
        public void Register(IDisposable disposable)
        {
            CheckShutdown();
            Callbacks.Add(()=>
            {
                disposable.Dispose();
                return Task.CompletedTask;
            });
        }
    }
}
