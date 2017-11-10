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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	public interface IEventQueue{
		Task Enqueue(object Event);
	}
	public interface IEventInstance
	{
		string Source { get; }
		string Type { get; }
		string Subscriber { get; }
		string EventId { get; }
		object Event { get; }
	}
	public interface IEventQueueProvider
	{
		IEventQueue GetQueue(
			string Source,
			string Type,
			string Subscriber,
			Func<IEventInstance,Task> EventEmiter,
			EventDeliveryPolicy Policy
			);
	}
	class SubscriberSet : IEventObservable
	{
		class Subscriber: IDisposable
		{
			public string Ident;
			public int Index;
			public EventDeliveryPolicy Policy;
			public Func<object,Task> Observer;
			public SubscriberSet Set;
			public void Dispose()
			{
				Set.RemoveSubscriber(Index);
			}
		}

		Subscriber[] _subscribers;
		ConcurrentDictionary<string, Subscriber> _subscriberDict;
		int _count;
		string Type { get; }
		EventSource Source { get; }

		public SubscriberSet(EventSource Source, string Type)
		{
			this.Source = Source;
			this.Type = Type;
		}

		public async Task Emit(IEvent Event)
		{
			var ss = _subscribers;
			if (ss== null)
				return;
			var l = ss.Length;
			//多线程下可能会有重复项目
			var hash = new HashSet<Subscriber>();
			for(var i=0;i<l;i++)
			{
				var s = ss[i];
				if (s == null)
					break;
				if (hash.Add(s))
					await s.Observer(Event);
			}
		}
		public async Task EmitQueueEvent(IEventInstance ei)
		{
			if (!_subscriberDict.TryGetValue(ei.Subscriber, out var s))
				throw new ArgumentException($"事件源{Source.Name}的类型{Type}中找不到事件处理器:{ei.Subscriber},事件:{ei.EventId}");
			await s.Observer(ei.Event);
		}
		public IDisposable Subscribe<TEvent>(string Ident,Func<TEvent, Task> observer,EventDeliveryPolicy Policy)
			where TEvent:class,IEvent
		{
			Func<object, Task> callback;
			if (Policy == EventDeliveryPolicy.NoGuarantee)
			{
				callback = o =>
				{
					return observer((TEvent)o);
				};
			}
			else {
				if (Ident.IsNullOrWhiteSpace())
					throw new ArgumentException("需要提供订阅ID");
				var queue = Source.GetEventQueue(Type, Ident, Policy);
				callback = queue.Enqueue;
			}
			lock (this)
			{
				var l = _subscribers?.Length ?? 0;
				if (_count == l)
					Array.Resize(ref _subscribers, (l == 0 ? 16 : l) * 2);
				var subscriber = new Subscriber
				{
					Ident=Ident,
					Policy=Policy,
					Index = _count,
					Observer = callback,
					Set = this
				};
				_subscribers[_count] = subscriber;
				_count++;

				if (Policy != EventDeliveryPolicy.NoGuarantee)
					_subscriberDict[Ident] = subscriber;

				return subscriber;
			}
		}
		void RemoveSubscriber(int Index)
		{
			lock (this)
			{
				if(Index<0 || Index>=_count)
					throw new ArgumentException();
				var last = _count - 1;
				var s = _subscribers[last];
				if (Index < last)
                {
                    _subscribers[Index] = s;
                    s.Index = Index;
                }
                _subscribers[last] = null;
				_count = last;
				_subscriberDict.TryRemove(s.Ident, out var ts);
			}
		}
	}
	class EventSource : IEventSource
	{
		public ConcurrentDictionary<string, SubscriberSet> TypedSubscribers { get; } = new ConcurrentDictionary<string, SubscriberSet>();
		public SubscriberSet UntypedSubscribers { get; }
		public EventManager Manager { get; }
		public string Name { get; }
		public EventSource(EventManager Manager,string Name)
		{
			this.Manager = Manager;
			this.Name = Name;
			UntypedSubscribers = new SubscriberSet(this,null);
		}
		public async Task Emit(IEvent Event)
		{
			SubscriberSet ss;
			if (TypedSubscribers.TryGetValue(Event.EventType, out ss))
				await ss.Emit(Event);

			await UntypedSubscribers.Emit(Event);
		}
		public async Task EmitQueueEvent(IEventInstance ei)
		{
			if (ei.Type.IsNullOrWhiteSpace())
				await UntypedSubscribers.EmitQueueEvent(ei);
			else if (TypedSubscribers.TryGetValue(ei.Type, out var s))
				await s.EmitQueueEvent(ei);
			else
				throw new ArgumentException($"事件源{Name}中找不到事件类型:{ei.Type}, 订阅器:{ei.Subscriber},事件:{ei.EventId}");
		}
		public IEventObservable GetObservable(string Type)
		{
			if (Type == null)
				return UntypedSubscribers;

			SubscriberSet ss;
			if (TypedSubscribers.TryGetValue(Type, out ss))
				return ss;
			ss = new SubscriberSet(this,Type);
			return TypedSubscribers.GetOrAdd(Type, ss);
		}
		public IEventQueue GetEventQueue(string Type,string SubscriberIdent, EventDeliveryPolicy Polic)
		{
			return Manager.GetEventQueue(Name,Type,SubscriberIdent, Polic);
		}
	}
	public class EventManager :
		IEventEmitter,
		ISourceResolver
	{
		ConcurrentDictionary<string, EventSource> EventSources { get; } = new ConcurrentDictionary<string, EventSource>();
		IEventQueueProvider EventQueueProvider { get; }
		public EventManager(IEventQueueProvider EventQueueProvider)
		{
			this.EventQueueProvider = EventQueueProvider;
		}

		Task EmitEvent(
            ConcurrentDictionary<string, EventSource> ess,
			IEvent Event
            )
        {
            EventSource es;
			if (ess.TryGetValue(Event.EventSource, out es))
                return es.Emit(Event);
            else
                return Task.CompletedTask;
        }
        public Task Emit(IEvent Event,bool SyncMode)
		{
            if (Event == null)
                throw new ArgumentNullException();
            if (SyncMode)
                return EmitEvent(EventSources, Event);
            else
            {
                Task.Run(() => EmitEvent(EventSources,  Event));
                return Task.CompletedTask;
            }
		}
		public async Task EmitQueueEvent(IEventInstance ei)
		{
			if (EventSources.TryGetValue(ei.Source, out var es))
				await es.EmitQueueEvent(ei);
			else
				throw new ArgumentException($"找不到事件源{ei.Source}, 事件类型:{ei.Type}，订阅器:{ei.Subscriber},事件:{ei.EventId}");
		}
		public IEventQueue GetEventQueue(string Source,string Type, string SubscriberIdent, EventDeliveryPolicy Policy)
		{
			if (EventQueueProvider == null)
				throw new NotSupportedException();
			return EventQueueProvider.GetQueue(
				Source ,
				Type ,
				SubscriberIdent,
				EmitQueueEvent,
				Policy
				);
		}
		public IEventSource GetSource(string Name)
		{
			EventSource es;
			if (EventSources.TryGetValue(Name, out es))
				return es;
			es = new EventSource(this,Name);
			return EventSources.GetOrAdd(Name, es);
		}
	}
}
