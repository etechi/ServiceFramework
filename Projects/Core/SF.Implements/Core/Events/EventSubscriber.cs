using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	public class EventSubscriber<T> : IEventSubscriber<T>,IDisposable
	{
		IDisposable _Disposable;
		object EventHandler;
		public EventSubscriber(ISourceResolver SourceResolver)
		{
			var pair = typeof(T).FullName.LastSplit2('.');
			var observer = SourceResolver.GetSource(pair.Item1)?.GetObservable(pair.Item2);
			_Disposable = observer.Subscribe(async o =>
			 {
				  var eh = EventHandler;
				  if (eh == null)
					  return;
				  var ol = EventHandler as List<Func<T, Task>>;
				  if (ol == null)
					  await ((Func<T, Task>)EventHandler)((T)o);
				  else
					  foreach (var h in ol)
						  await h((T)o);

			 });
		}

		public void Dispose()
		{
			Disposable.Release(ref _Disposable);
		}

		public void OnEvent(Func<T, Task> Callback)
		{
			if (EventHandler == null)
				EventHandler = Callback;
			else
			{
				var ol = EventHandler as List<Func<T, Task>>;
				if (ol == null)
					EventHandler = new List<Func<T, Task>> { (Func<T, Task>)EventHandler, Callback };
				else
					ol.Add(Callback);
			}
		}
	}

}
