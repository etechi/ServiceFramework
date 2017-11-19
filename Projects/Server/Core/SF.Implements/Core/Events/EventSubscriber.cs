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
	public class EventSubscriber<TEvent> : IEventSubscriber<TEvent>,IDisposable where TEvent:class,IEvent
	{
		IDisposable _Disposable;
		object EventHandler;
		public EventSubscriber(IEventSubscribeService SubscribeService,
			string EventSource,
			string EventType, 
			string SubscriberIdent=null,
			EventDeliveryPolicy Policy=EventDeliveryPolicy.NoGuarantee
			)
		{
			if(EventSource==null && EventType==null)
			{
				var et = typeof(TEvent);
				EventSource = et.Namespace;
				EventType = et.Name;
			}
			var observer = SubscribeService.GetObservable(EventSource,EventType);

			_Disposable = observer.Subscribe(
				SubscriberIdent,
				Policy,
				new DelegateEventObserver<TEvent>(
				 async o =>
				 {
					  var eh = EventHandler;
					  if (eh == null)
						  return;
					  var ol = EventHandler as List<Func<IEventInstance<TEvent>, Task>>;
					  if (ol == null)
						  await ((Func<IEventInstance<TEvent>, Task>)EventHandler)(o);
					  else
						  foreach (var h in ol)
							  await h(o);

				 })
				 );
		}

		public void Dispose()
		{
			Disposable.Release(ref _Disposable);
		}

		public void Wait(Func<IEventInstance<TEvent>, Task> Callback)
		{
			if (EventHandler == null)
				EventHandler = Callback;
			else
			{
				var ol = EventHandler as List<Func<IEventInstance<TEvent>, Task>>;
				if (ol == null)
					EventHandler = new List<Func<IEventInstance<TEvent>, Task>> { (Func<IEventInstance<TEvent>, Task>)EventHandler, Callback };
				else
					ol.Add(Callback);
			}
		}
	}

}
