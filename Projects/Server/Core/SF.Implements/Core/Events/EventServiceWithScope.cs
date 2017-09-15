using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SF.Core.Events
{	
	public static class TaskEventService
	{
        public static IDisposable Start(
            this ISourceResolver SourceResolver,
            string EventSourceName,
			string EventTypeName,
            Func<object, Task> EventCallback,
            Action CleanupCallback=null,
            Func<bool> IsStopped=null
            )
		{
            var disposable = SourceResolver
                .GetObservable(EventSourceName, EventTypeName)
                .Subscribe(e =>
                {
                    if (IsStopped?.Invoke() ?? false)
                        return Task.CompletedTask;
                    return EventCallback(e);
                });
            if (CleanupCallback == null)
                return disposable;

            return Disposable.Combine(
                disposable,
                Disposable.FromAction(CleanupCallback)
                );
		}
       

    }
	
}
