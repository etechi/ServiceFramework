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

using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	public interface IEvent
	{
		long? EventId { get; }
		long? ServiceId { get; }
		string EventSource { get; }
		string EventType { get; }
		DateTime EventTime { get; }
	}
	public abstract class CommonEvent : IEvent
	{
		public DateTime EventTime { get; set; }
		public long? EventId { get; set; }
		public string EventSource { get; set; }
		public string EventType { get; set; }
		public long? ServiceId { get; set; }
	}

	public interface IEventValidator<T>
	{
		Task<bool> Validate(T Event);
	}

	public interface IEventSubscriber<T>
	{
		void Wait(Func<T,Task> Callback);
	}
	public enum EventDeliveryPolicy
	{
		[Comment("无保证")]
		NoGuarantee,
		[Comment("最多一次")]
		AtMostOnce,
		[Comment("最少一次")]
		AtLeastOnce,
		[Comment("刚好一次")]
		JustOnce
	}
	public interface IEventObservable
    {
        IDisposable Subscribe<TEvent>(string SubscriberIdent,Func<TEvent, Task> observer, EventDeliveryPolicy Policy)
			where TEvent:class,IEvent;
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
		Task Emit(IEvent Event,bool SyncMode=true);
	}
   
}
