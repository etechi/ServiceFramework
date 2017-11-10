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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	public class EventSubscriber<T> : IEventSubscriber<T>,IDisposable
	{
		IDisposable _Disposable;
		object EventHandler;
		public EventSubscriber(ISourceResolver SourceResolver,
			string EventSource,
			string EventType, 
			string SubscriberIdent=null,
			EventDeliveryPolicy Policy=EventDeliveryPolicy.NoGuarantee
			)
		{
			if (EventSource == null && EventType==null)
			{
				(EventSource,EventType) = typeof(T).FullName.LastSplit2('.');
			}
			else if(EventSource==null || EventType==null)
			{
				throw new ArgumentException($"必须同时设置事件源和类型");
			}
			var observer = SourceResolver.GetSource(EventSource)?.GetObservable(EventType);

			_Disposable = observer.Subscribe<IEvent>(
				SubscriberIdent,
				async o =>
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

				 },
				 Policy
				 );
		}

		public void Dispose()
		{
			Disposable.Release(ref _Disposable);
		}

		public void Wait(Func<T, Task> Callback)
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
