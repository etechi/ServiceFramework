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
	
	public static class EventSourceExtension
    {
		public static async Task Emit<TEvent>(this IEventEmitService Service,TEvent Event) where TEvent:class,IEvent
		{
			var e = await Service.Create(Event);
			await e.Commit();
		}
		public static IDisposable Subscribe<TEvent>(this IEventObservable Observable,Func<IEventInstance<TEvent>,Task> observer) where TEvent:class,IEvent
		{
			return Observable.Subscribe(null, EventDeliveryPolicy.NoGuarantee,new DelegateEventObserver<TEvent>(observer));
		}
		public static IEventObservable GetObservable(this IEventSubscribeService Resolver, string Source, string Type)
		{
			return Resolver.GetObservable(Source,Type);
		}
		
		public static IDisposable Subscribe<TEvent>(this IEventSubscribeService Resolver, string Source,string Type, Func<IEventInstance<TEvent>, Task> Callback)
			where TEvent:class,IEvent
		{
			return Resolver.GetObservable(Source,Type)
				.Subscribe(Callback);
		}
		public static IDisposable Subscribe<TEvent>(this IEventSubscribeService Resolver, string Source, Func<IEventInstance<TEvent>, Task> Callback)
			where TEvent : class, IEvent
		{
			return Resolver.Subscribe(Source, null, Callback);
		}
		
		public static IDisposable Filter<TSrcEvent,TDstEvent>(
			this IEventSubscribeService Resolver,
			string Source,
			string Type,
			IEventEmitService EmitService,
			Func<IEventInstance<TSrcEvent>, Task<TDstEvent>> Filter
			)
			where TSrcEvent: class, IEvent
			where TDstEvent: class, IEvent
		{
			return Resolver.Subscribe<TSrcEvent>(
                Source, 
                Type, 
                async se =>
                {
					var de=await EmitService.Create(await Filter(se));
					await de.Commit();
                }
                );
		}

	}

}
