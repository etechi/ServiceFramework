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

using SF.Sys.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Sys.Events
{

    public class EventEmitService : IEventEmitService
    {
        IIdentGenerator IdentGenerator { get; }

        IServiceProvider ServiceProvider { get; }
        IEventSubscribeService EventSubscribeService { get; }
        public EventEmitService(
            IServiceProvider ServiceProvider, 
            IIdentGenerator IdentGenerator, 
            IEventSubscribeService EventSubscribeService
            )
        {
            this.ServiceProvider = ServiceProvider;
            this.IdentGenerator = IdentGenerator;
            this.EventSubscribeService = EventSubscribeService;
        }
        class EventEmitter<TPayload> : IEventEmitter<TPayload>, IEvent<TPayload>
        {
            public IServiceProvider Services { get; internal set; }
            public long Id { get; set; }
            public string Topic { get; set; }
            public TPayload Payload { get; set; }
            public EventManager EventManager { get; set; }
            public List<(IEventObserver<TPayload>, object)> Results { get; set; }

            public async Task Cancel(Exception Exception)
            {
                foreach (var (o, c) in Results)
                    await o.Cancel(this, c, Exception);
            }

            void CommitAsync()
            {
                Task.Run(async () =>
                {
                    foreach (var o in EventManager.Router.GetObservers<TPayload>(Topic, false))
                    {
                        var ctx = await o.Prepare(this);
                        await o.Commit(this, ctx);
                    }
                });
            }
            public async Task Commit()
            {
                foreach (var (o, c) in Results)
                    await o.Commit(this, c);
                CommitAsync();
            }

        }
        public async Task<IEventEmitter> Create<TPayload>(string Topic, TPayload Payload)
        {
            var EventManager = (EventManager)this.EventSubscribeService;
            var Id = await IdentGenerator.GenerateAsync("event:" + Topic);

            var e = new EventEmitter<TPayload>
            {
                Id = Id,
                Payload = Payload,
                Topic = Topic,
                EventManager = EventManager,
                Services = ServiceProvider
            };
            var obs = EventManager.Router.GetObservers<TPayload>(Topic, true);
            var ps = new List<(IEventObserver<TPayload>, object)>();
            foreach (var o in obs)
                ps.Add((o, await o.Prepare(e)));
            e.Results = ps;
            return e;
        }
    }

    public class EventManager :
		IEventSubscribeService
	{
		
		public TopicRouter Router { get; }

		IEventQueueProvider EventQueueProvider { get; }

		public EventManager(IEventQueueProvider EventQueueProvider)
		{
			this.Router = new TopicRouter(this);
			this.EventQueueProvider = EventQueueProvider;
		}
		public IDisposable Subscribe<TPayload>(
			string TopicFilter,
			string SubscriberIdent,
			EventDeliveryPolicy Policy,
			IEventObserver<TPayload> Observer
			)
		{
			return Router.Subscribe<TPayload>(TopicFilter, SubscriberIdent, Policy, Observer);
		}
	

		public IEventQueue<TPayload> GetEventQueue<TPayload>(
			string Path, 
			string SubscriberIdent, 
			EventDeliveryPolicy Policy,
			IEventObserver<TPayload> Observer
			) 
		{
			if (EventQueueProvider == null)
				throw new NotSupportedException();
			return EventQueueProvider.GetQueue(
				Path,
				SubscriberIdent,
				Policy,
				Observer
				);
		}
		
		
	}
}
