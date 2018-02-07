
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace SF.Sys.Events
{
	
	public interface IEventQueue
	{
		void Enqueue(IEvent Event);
	}

	class TopicDirectory :
		ConcurrentDictionary<string, TopicDirectory>
	{
		public string Name { get; }
		public string Path { get; }

		public TopicDirectory(EventManager EventManager, string Name,string Path)
		{
			this.EventManager = EventManager;
			this.Name = Name;
			this.Path = Path;
		}

		SubscriptionSet _SyncSubscribers;

		SubscriptionSet _AsyncSubscribers;

		EventManager EventManager { get; }

		public bool HasAsyncSubscriber => _AsyncSubscribers.Count > 0;

		public IEnumerable<Subscription<TPayload>> GetObservers<TPayload>(bool Sync)
		{
			var ss = Sync ? _SyncSubscribers : _AsyncSubscribers;
			if (ss == null)
				yield break;
			foreach (var s in ss)
				yield return (Subscription<TPayload>)s;
		}


		public IDisposable Subscribe<TPayload>(
			string[] SegFilters,
			string Ident,
			EventDeliveryPolicy Policy,
			IEventObserver<TPayload> Observer
			)
		{
			lock (this)
			{
				if (Policy != EventDeliveryPolicy.NoGuarantee && Policy != EventDeliveryPolicy.TryBest)
				{
					if (Ident.IsNullOrWhiteSpace())
						throw new ArgumentException("需要提供订阅ID");
					Observer = EventManager.GetEventQueue(Path, Ident, Policy, Observer);
				}

				var subscriber = new Subscription<TPayload>
				{
					SegFilters= SegFilters,
					Policy = Policy,
					Observer = Observer,
					Dictionary = this
				};

				switch (Policy)
				{
					case EventDeliveryPolicy.NoGuarantee:
					case EventDeliveryPolicy.TryBest:
						if (_AsyncSubscribers == null)
							_AsyncSubscribers = new SubscriptionSet();
						subscriber.Index = _AsyncSubscribers.Add(subscriber);
						break;
					default:
						if (_SyncSubscribers == null)
							_SyncSubscribers = new SubscriptionSet();
						subscriber.Index = _SyncSubscribers.Add(subscriber);
						break;
				}

				return subscriber;
			}
		}


		internal void RemoveSubscriber(int Index, EventDeliveryPolicy Policy)
		{
			lock (this)
			{
				if (Policy == EventDeliveryPolicy.NoGuarantee || Policy == EventDeliveryPolicy.TryBest)
				{
					_AsyncSubscribers.Remove(Index);
				}
				else
				{
					_SyncSubscribers.Remove(Index);
				}
			}
		}
	}
}
 