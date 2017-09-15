using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	
	public static class EventSourceExtension
    {
		public static IEventObservable GetObservable(this ISourceResolver Resolver, string Source, string Type)
		{
			return Resolver.GetSource(Source).GetObservable(Type);
		}
		public static IDisposable Subscribe(this ISourceResolver Resolver, string Source,string Type, Func<object,Task> Callback)
		{
			return Resolver.GetObservable(Source,Type)
				.Subscribe(Callback);
		}
		public static IDisposable Subscribe(this ISourceResolver Resolver, string Source, Func<object, Task> Callback)
		{
			return Resolver.Subscribe(Source, null, Callback);
		}

		public static IDisposable Filter(
			this ISourceResolver Resolver,
			string Source,
			string Type,
			IEventEmitter Emiter,
			Func<object, object> Filter,
            bool OutputSyncMode=false
			)
		{
			return Resolver.Subscribe(
                Source, 
                Type, 
                e =>
                {
                    var re = Filter(e);
                    if (re == null)
                        return Task.CompletedTask;
                    return Emiter.Emit(re, OutputSyncMode);
                }
                );
		}

	}

}
