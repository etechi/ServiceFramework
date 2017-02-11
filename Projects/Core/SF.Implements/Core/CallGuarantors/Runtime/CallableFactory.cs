using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.DI;
using System.Threading;

namespace SF.Core.CallGuarantors.Runtime
{
	public class CallableFactory
	{
		Dictionary<string, Func<IServiceProvider, ICallable>> Creators { get; }
		public CallableFactory(IEnumerable<ICallableDefination> callables)
		{
			Creators = callables.ToDictionary(c => c.Type.FullName, c => c.CallableCreator);
		}
		public bool Exists(string Name)
		{
			return Creators.ContainsKey(Name);
		}
		public ICallable Create(IServiceProvider ServiceProvider, string Name)
		{
			var f = Creators.Get(Name);
			if (f == null)
				throw new ArgumentException("找不到调用接口:" + Name);
			return f(ServiceProvider);
		}
	}
}
