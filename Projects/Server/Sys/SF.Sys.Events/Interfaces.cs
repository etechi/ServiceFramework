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

namespace SF.Sys.Events
{
	public interface IEvent
	{
		DateTime Time { get; }
		string Source { get; }
		string Type { get; }
		string TraceIdent { get; }
		string Target { get; }
	}

	public class CommonEvent : IEvent
	{
		public CommonEvent()
		{
			var t = GetType();
			Source = t.Namespace;
			Type = t.Name;
		}
		public DateTime Time { get; set; }
		public string Target { get; set; }
		public string Source { get; set; }
		public string Type { get; set; }
		public string TraceIdent { get; set; }
	}
	public interface IEventInstance<TEvent>
		where TEvent:IEvent
	{
		long Id { get; }
		TEvent Event { get; }
	}

	
	public interface IEventEmitter 
	{
		long Id { get; }
		IEvent Event { get; }
		Task Cancel(Exception Exception);
		Task Commit();
	}

	public interface IEventValidator<T>
	{
		Task<bool> Validate(T Event);
	}

	public interface IEventSubscriber<TEvent> where TEvent:IEvent
	{
		void Wait(
			Func<IEventInstance<TEvent>,Task> Callback,
			EventDeliveryPolicy Policy = EventDeliveryPolicy.NoGuarantee,
			string EventSource = null,
			string EventType = null,
			string SubscriberIdent = null
			);
	}
	public interface IEventObserver<TEvent> where TEvent:IEvent
	{
		Task<object> Prepare(IEventInstance<TEvent> EventInstance);
		Task Commit(IEventInstance<TEvent> EventInstance,object Context);
		Task Cancel(IEventInstance<TEvent> EventInstance, object Context, Exception Exception);
	}
	public interface IEventObservable
    {
        IDisposable Subscribe<TEvent>(
			string SubscriberIdent,
			EventDeliveryPolicy Policy,
			IEventObserver<TEvent> Observer
			)
			where TEvent:class,IEvent;
    }

	public interface IEventSubscribeService
	{
		IEventObservable GetObservable(string Source,string Type);
	}


	public interface IEventEmitService
	{
		Task<IEventEmitter> Create<TEvent>(TEvent Event) where TEvent : IEvent;

	}
   
}
