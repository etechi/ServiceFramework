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

using SF.Core.Times;
using SF.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Events
{
	class CommonEventInstance<TEvent> : IEventInstance<TEvent>
		where TEvent : IEvent
	{
		public long Id { get; set; }

		public TEvent Event { get; set; }
	}

	public class EventManager :
		IEventEmitService,
		IEventSubscribeService
	{

		ConcurrentDictionary<(string, string), EventObservable> SourceTypeLimitedObservables { get; } = new ConcurrentDictionary<(string, string), EventObservable>();
		ConcurrentDictionary<string, EventObservable> TypeLimitedObservables { get; } = new ConcurrentDictionary<string, EventObservable>();
		ConcurrentDictionary<string, EventObservable> SourceLimitedObservables { get; } = new ConcurrentDictionary<string, EventObservable>();
		EventObservable UnlimitedObservables { get; }

		IEventQueueProvider EventQueueProvider { get; }
		IIdentGenerator IdentGenerator { get; }
		public EventManager(IEventQueueProvider EventQueueProvider, IIdentGenerator IdentGenerator)
		{
			this.EventQueueProvider = EventQueueProvider;
			this.IdentGenerator = IdentGenerator;
			this.UnlimitedObservables = new EventObservable(this, null, null);
		}

		public IEventObservable GetObservable(string Source, string Type)
		{
			if (Source == null)
			{
				if (Type == null)
					return UnlimitedObservables;
				else
					return TypeLimitedObservables.GetOrAdd(Type, t => new EventObservable(this, null, t));
			}
			else if (Type == null)
				return SourceLimitedObservables.GetOrAdd(Source, s => new EventObservable(this, s, null));
			else
				return SourceTypeLimitedObservables.GetOrAdd((Source,Type), k => new EventObservable(this, k.Item1, k.Item2));
		}


		IEnumerable<EventObservable> GetObservables(string Source,string Type)
		{
			if (SourceTypeLimitedObservables.TryGetValue((Source, Type), out var s1))
				yield return s1;
			if (TypeLimitedObservables.TryGetValue(Type, out var s2))
				yield return s2;
			if (SourceLimitedObservables.TryGetValue(Source, out var s3))
				yield return s3;
			yield return UnlimitedObservables;
		}

		IEnumerable<IEventObserver<TEvent>> GetEventObservers<TEvent>(string Source,string Type,bool Sync) where TEvent:IEvent
		{
			foreach (var eo in GetObservables(Source, Type))
				foreach (var o in eo.GetObservers<TEvent>(Sync))
					yield return o;
		}

		public IEventQueue<TEvent> GetEventQueue<TEvent>(
			string Source,
			string Type, 
			string SubscriberIdent, 
			EventDeliveryPolicy Policy,
			IEventObserver<TEvent> Observer
			) where TEvent:IEvent
		{
			if (EventQueueProvider == null)
				throw new NotSupportedException();
			return EventQueueProvider.GetQueue(
				Source ,
				Type ,
				SubscriberIdent,
				Policy,
				Observer
				);
		}
		
		class EventEmitter<TEvent> : IEventEmitter where TEvent : IEvent
		{
			public EventManager EventManager { get; set; }
			public IEventInstance<TEvent> EventInstance { get; set; }
			public List<(IEventObserver<TEvent>, object)> Results { get; set; }

			public IEvent Event => EventInstance.Event;
			public long Id => EventInstance.Id;


			public async Task Cancel(Exception Exception)
			{
				foreach (var (o, c) in Results)
					await o.Cancel(EventInstance, c, Exception);
			}

			void CommitAsync()
			{
				Task.Run(async () =>
				{
					var Event = EventInstance.Event;
					foreach (var o in EventManager.GetEventObservers<TEvent>(Event.Source, Event.Type, false))
					{
						var ctx =await o.Prepare(EventInstance);
						await o.Commit(EventInstance, ctx);
					}
				});
			}
			public async Task Commit()
			{
				foreach (var (o, c) in Results)
					await o.Commit(EventInstance,c);
				CommitAsync();
			}
		
		}
		public async Task<IEventEmitter> Create<TEvent>(TEvent Event) where TEvent : IEvent
		{
			var obs = GetObservables(Event.Source, Event.Type).ToArray();

			var ei = new CommonEventInstance<TEvent>
			{
				Id = await IdentGenerator.GenerateAsync($"Event/{Event.Source}/{Event.Type}"),
				Event = Event
			};

			var ps = new List<(IEventObserver<TEvent>, object)>();

			foreach (var o in GetEventObservers<TEvent>(Event.Source, Event.Type, true))
				ps.Add((o, await o.Prepare(ei)));

			return new EventEmitter<TEvent>
			{
				EventInstance=ei,
				Results=ps,
				EventManager = this
			};
		}
	}
}
