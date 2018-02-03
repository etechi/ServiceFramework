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

using SF.Sys.Data;
using SF.Sys.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.ServiceFeatures
{
	public class ServiceFeatureControlService : IServiceFeatureControlService
	{
		IServiceProvider ServiceProvider { get; }
		ILogger Logger { get; }
		IDataScope DataScope { get; }
		SF.Sys.NetworkService.IInvokeContext InvokeContext { get; }
		public ServiceFeatureControlService(
			IServiceProvider ServiceProvider,
			ILogger<ServiceFeatureControlService> Logger,
			SF.Sys.NetworkService.IInvokeContext InvokeContext,
			IDataScope DataScope
			)
		{
			this.DataScope = DataScope;
			this.InvokeContext = InvokeContext;
			this.Logger = Logger;
			this.ServiceProvider = ServiceProvider;
		}
		public Task<string> Init(string Id=null)
		{
			return DataScope.Use("服务初始化:" + Id, async ctx =>
			{
				var u = new Uri(InvokeContext.Request.Uri);
				var args = u.ParseQuery().ToDictionary(p => p.key, p => p.value);
				await ServiceProvider.InitServices(Id, args);
				return "OK";
			},
			DataContextFlag.None,
			System.Data.IsolationLevel.Serializable
			);
		}
	}

}
