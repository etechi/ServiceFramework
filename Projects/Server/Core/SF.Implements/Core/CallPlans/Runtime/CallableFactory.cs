using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using System.Threading;

namespace SF.Core.CallPlans.Runtime
{
	public class CallableFactory
	{
		Dictionary<string, Func<IServiceProvider, long?, ICallable>> Creators { get; }
		public CallableFactory(IEnumerable<ICallableDefination> callables)
		{
			Creators = callables.ToDictionary(c => c.Type, c => c.CallableCreator);
		}
		public bool Exists(string Name)
		{
			return Creators.ContainsKey(Name);
		}
		public ICallable Create(IServiceProvider ServiceProvider, string Name,long? Id)
		{
			var f = Creators.Get(Name);
			if (f == null)
				throw new ArgumentException("找不到调用接口:" + Name);
			return f(ServiceProvider, Id);
		}
	}
}
