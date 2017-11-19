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
using System.Threading.Tasks;
using System.Collections;

namespace SF.Core.Events
{
	abstract class Subscriber : IDisposable
	{
		public int Index;
		public EventDeliveryPolicy Policy;
		public EventObservable Set;
		public virtual void Dispose()
		{
			Set.RemoveSubscriber(Index, Policy);
		}
	}
	class Subscriber<TEvent> : Subscriber
		where TEvent : IEvent
	{
		public IEventObserver<TEvent> Observer { get; set; }
		public override void Dispose()
		{
			base.Dispose();
			var d = Observer as IDisposable;
			if (d != null)
				d.Dispose();
		}
	}
	class SubscriberSet : IEnumerable<Subscriber>
	{
		Subscriber[] _Items;
		int _Count;
		public int Count => _Count;


		public int Add(Subscriber subscriber)
		{
			var l = _Items?.Length ?? 0;
			if (_Count == l)
				Array.Resize(ref _Items, (l == 0 ? 16 : l) * 2);
			var idx = _Count;
			_Items[_Count] = subscriber;
			_Count++;
			return idx;
		}

		public IEnumerator<Subscriber> GetEnumerator()
		{
			if (_Items == null)
				yield break;
			var l = _Items.Length;
			//多线程下可能会有重复项目
			var hash = new HashSet<Subscriber>();
			for (var i = 0; i < l; i++)
			{
				var s = _Items[i];
				if (s == null)
					break;
				if (hash.Add(s))
					yield return s;
			}
		}

		public void Remove(int Index)
		{
			if (Index < 0 || Index >= _Count)
				throw new ArgumentException();
			var last = _Count - 1;
			var s = _Items[last];
			if (Index < last)
			{
				_Items[Index] = s;
				s.Index = Index;
			}
			_Items[last] = null;
			_Count = last;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
	class EventObservable : IEventObservable
	{

		SubscriberSet _SyncSubscribers;

		SubscriberSet _AsyncSubscribers;

		string Source { get; }
		string Type { get; }
		EventManager EventManager { get; }
		
		public EventObservable(EventManager EventManager,string Source,string Type)
		{
			this.EventManager = EventManager;
			this.Source = Source;
			this.Type = Type;
		}

		public bool HasAsyncSubscriber => _AsyncSubscribers.Count > 0;

		public IEnumerable<IEventObserver<TEvent>> GetObservers<TEvent>(bool Sync) where TEvent:IEvent
		{
			var ss = Sync ? _SyncSubscribers : _AsyncSubscribers;
			if (ss == null)
				yield break;
			foreach (var s in ss)
				yield return ((Subscriber<TEvent>)s).Observer;
		}

		
		public IDisposable Subscribe<TEvent>(
			string Ident, 
			EventDeliveryPolicy Policy,
			IEventObserver<TEvent> Observer
			)
			where TEvent:class,IEvent
		{
			lock (this)
			{
				if (Policy != EventDeliveryPolicy.NoGuarantee && Policy != EventDeliveryPolicy.TryBest)
				{
					if (Ident.IsNullOrWhiteSpace())
						throw new ArgumentException("需要提供订阅ID");
					Observer = EventManager.GetEventQueue(Source, Type, Ident, Policy, Observer);
				}

				var subscriber = new Subscriber<TEvent>
				{
					Policy = Policy,
					Observer = Observer,
					Set = this
				};

				switch (Policy)
				{
					case EventDeliveryPolicy.NoGuarantee:
					case EventDeliveryPolicy.TryBest:
						if (_AsyncSubscribers == null)
							_AsyncSubscribers = new SubscriberSet();
						subscriber.Index = _AsyncSubscribers.Add(subscriber);
						break;
					default:
						if (_SyncSubscribers == null)
							_SyncSubscribers = new SubscriberSet();
						subscriber.Index = _SyncSubscribers.Add(subscriber);
						break;
				}

				return subscriber;
			}
		}

		
		internal void RemoveSubscriber(int Index,EventDeliveryPolicy Policy)
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
