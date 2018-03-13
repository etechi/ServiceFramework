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
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using SF.Sys.Logging;
using SF.Sys.NetworkService;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys;
using SF.Sys.Services;

namespace SF.Common.FrontEndContents.Runtime
{
	public class ServiceDataProvider : IDataProvider
	{
		public ILogger Logger { get; }
		public IServiceInvokerProvider ServiceInvokerProvider { get; }
		public IServiceProvider ServiceProvider { get; }
		public SF.Sys.NetworkService.Metadata.Library NetLibrary { get; }
		public ServiceDataProvider(
			IServiceProvider ServiceProvider,
			ILogger<ServiceDataProvider> Logger, 
			IServiceInvokerProvider ServiceInvokerProvider,
			SF.Sys.NetworkService.Metadata.Library NetLibrary
			)
		{
			this.ServiceProvider = ServiceProvider;
			this.Logger = Logger;
			this.NetLibrary = NetLibrary;
			this.ServiceInvokerProvider = ServiceInvokerProvider;
		}

		public Task<object> Load(IContent content, string contentConfig)
		{
			return Load(content.ProviderConfig, contentConfig);
		}
		static void ObjectMergeTo(JObject v, JObject o)
		{
			foreach(var p in v)
			{
				if (o.TryGetValue(p.Key, out var ov))
				{
					if (ov is JObject ovo && p.Value is JObject pvo)
					{
						ObjectMergeTo(pvo, ovo);
					}
					else
						o[p.Key] = p.Value;
				}
				else
					o[p.Key] = p.Value;
			}
		}
		static ConcurrentDictionary<Type, Func<Task, object>> TaskValueExtectors { get; }
			= new ConcurrentDictionary<Type, Func<Task, object>>();
		static Expression ArgTask { get; } = Expression.Parameter(typeof(Task));
		static object ExtractTaskResult(Task result)
		{
			var retType = result.GetType();
			if (!TaskValueExtectors.TryGetValue(retType, out var f))
			{
				if (retType.IsGenericTypeOf(typeof(Task<>)))
					f = ArgTask.To(retType)
						.GetMember(nameof(Task<int>.Result))
						.To<object>()
						.Compile<Func<Task, object>>(ArgTask);
				else 
					f = r => null;
				f = TaskValueExtectors.GetOrAdd(retType, f);
			}
			return f(result);
		}
		public async Task<object> Load(string contentConfig, string blockConfig)
		{
			if (string.IsNullOrWhiteSpace(contentConfig))
				return null;

			var ctnCfg = (JObject) Newtonsoft.Json.JsonConvert.DeserializeObject(contentConfig);
			if (!string.IsNullOrEmpty(blockConfig))
				ObjectMergeTo(
					(JObject)Newtonsoft.Json.JsonConvert.DeserializeObject(blockConfig),
					ctnCfg
					);
			if (!ctnCfg.TryGetValue("service", out var svcToken))
				return null;
			var svc = svcToken.ToString();

			if (!ctnCfg.TryGetValue("method", out var methodToken))
				return null;

			var method = methodToken.ToString();

			if (svc.IsNullOrEmpty() || method.IsNullOrEmpty())
				return null;

			if (!ctnCfg.TryGetValue("args", out var argToken))
				return Task.FromResult<object>(null);

			long? sid=null;
			if (ctnCfg.TryGetValue("scope", out var argSid))
				sid = argSid.ToString().ToInt64();

			var arg = argToken.ToString();

			var (svcType, mthd) = NetLibrary.FindMethod(svc, method);
			if (svcType == null || mthd == null)
				throw new PublicArgumentException($"找不到服务{svc}或方法{method}");

			var re=await ServiceInvokerProvider.Resolve(svcType.GetSysType(), mthd.GetSysMethod()).InvokeAsync(
				ServiceProvider,
				arg
				);
			
			//var cfgs = Json.Parse<Dictionary<string, string>>(contentConfig);
			//if (!string.IsNullOrWhiteSpace(blockConfig))
			//{
			//	var block_cfgs = Json.Parse<Dictionary<string, string>>(blockConfig);
			//	foreach (var p in block_cfgs)
			//		cfgs[p.Key] = p.Value;
			//}
			return re;
		}
	}

}
