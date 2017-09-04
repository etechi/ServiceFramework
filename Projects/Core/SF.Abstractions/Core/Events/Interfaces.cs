using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	public interface IEventSubscriber<T>
	{
		void OnEvent(Func<T,Task> Callback);
	}

	public interface IEventObservable
    {
        IDisposable Subscribe(Func<object,Task> observer);
    }
    public interface IEventSource
    {
		IEventObservable GetObservable(string Type);
    }

	public interface ISourceResolver
	{
		IEventSource GetSource(string Name);
	}

	public interface IEventEmitter
	{
		Task Emit(object Event,bool SyncMode);
	}
   
}
