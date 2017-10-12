using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.Entities;
using SF.Core;
using SF.Core.NetworkService;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Linq.Expressions;

namespace SF.Management.FrontEndContents.Runtime
{
	public class ServiceDataProvider : IDataProvider
	{
		public ILogger<ServiceDataProvider> Logger { get; }
		public IServiceInvoker ServiceInvoker { get; }
		public IServiceProvider ServiceProvider { get; }
		public ServiceDataProvider(IServiceProvider ServiceProvider,ILogger<ServiceDataProvider> Logger, IServiceInvoker ServiceInvoker)
		{
			this.ServiceProvider = ServiceProvider;
			this.Logger = Logger;
			this.ServiceInvoker = ServiceInvoker;
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
		static Expression ArgTask { get; } = Expression.Parameter(typeof(Expression));
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
			var re=ServiceInvoker.Invoke(
				ServiceProvider,
				sid,
				svc,
				method,
				arg
				);

			if (re is Task t)
			{
				await t;
				return ExtractTaskResult(t);
			}
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
