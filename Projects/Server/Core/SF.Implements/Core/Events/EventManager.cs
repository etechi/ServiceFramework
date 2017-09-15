using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	class SubscriberSet : IEventObservable
	{
		class Subscriber: IDisposable
		{
			public int Index;
			public Func<object,Task> Observer;
			public SubscriberSet Set;
			public void Dispose()
			{
				Set.RemoveSubscriber(Index);
			}
		}

		Subscriber[] _subscribers;
		int _count;

		public async Task Emit(object Event)
		{
			var ss = _subscribers;
			if (ss== null)
				return;
			var l = ss.Length;
			var hash = new HashSet<Subscriber>();
			for(var i=0;i<l;i++)
			{
				var s = ss[i];
				if (s == null)
					break;
				if(hash.Add(s))
					try
					{
						await s.Observer(Event);
					}
					catch { }
			}
		}

		public IDisposable Subscribe(Func<object, Task> observer)
		{
			lock (this)
			{
				var l = _subscribers?.Length ?? 0;
				if (_count == l)
					Array.Resize(ref _subscribers, (l == 0 ? 16 : l) * 2);
				var subscriber = new Subscriber
				{
					Index = _count,
					Observer = observer,
					Set = this
				};
				_subscribers[_count] = subscriber;
				_count++;
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
                if (Index < last)
                {
                    var l = _subscribers[last];
                    _subscribers[Index] = l;
                    l.Index = Index;
                }
                _subscribers[last] = null;
				_count = last;
			}
		}
	}
	class EventSource : IEventSource
	{
		public ConcurrentDictionary<string, SubscriberSet> TypedSubscribers { get; } = new ConcurrentDictionary<string, SubscriberSet>();
		public SubscriberSet UntypedSubscribers { get; } = new SubscriberSet();
		public async Task Emit(object Event,string Type)
		{
			SubscriberSet ss;
			if (TypedSubscribers.TryGetValue(Type, out ss))
				await ss.Emit(Event);

			await UntypedSubscribers.Emit(Event);
		}

		public IEventObservable GetObservable(string Type)
		{
			if (Type == null)
				return UntypedSubscribers;

			SubscriberSet ss;
			if (TypedSubscribers.TryGetValue(Type, out ss))
				return ss;
			ss = new SubscriberSet();
			return TypedSubscribers.GetOrAdd(Type, ss);
		}
	}
	public class EventManager :
		IEventEmitter,
		ISourceResolver
	{
		ConcurrentDictionary<string, EventSource> EventSources { get; } = new ConcurrentDictionary<string, EventSource>();

        Task EmitEvent(
            ConcurrentDictionary<string, EventSource> EventSources,
			object Event
            )
        {
            EventSource es;
			var namePair = Event.GetType().FullName.LastSplit2('.');
			if (EventSources.TryGetValue(namePair.Item1, out es))
                return es.Emit(Event, namePair.Item2);
            else
                return Task.CompletedTask;
        }
        public Task Emit(object Event,bool SyncMode)
		{
            if (Event == null)
                throw new ArgumentNullException();
            if (SyncMode)
                return EmitEvent(EventSources, Event);
            else
            {
                Task.Run(() => EmitEvent(EventSources, Event));
                return Task.CompletedTask;
            }
		}

		public IEventSource GetSource(string Name)
		{
			EventSource es;
			if (EventSources.TryGetValue(Name, out es))
				return es;
			es = new EventSource();
			return EventSources.GetOrAdd(Name, es);
		}
	}
}
