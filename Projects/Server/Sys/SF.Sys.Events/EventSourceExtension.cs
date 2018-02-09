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
	public static class TypeTopic<TPayload>
	{
		public static string Name { get; }
		static string GetTypeTopic(Type type)
		{
			var topic = type.Namespace;
			if (type.IsGenericType)
			{
				topic += "/" + type.GetGenericTypeDefinition().Name;
				foreach (var d in type.GetGenericArguments())
					topic += "/" + GetTypeTopic(d);
			}
			else
				topic += "/" + type.Name;
			return topic;
		}
		static TypeTopic()
		{
			Name = GetTypeTopic(typeof(TPayload));
		}
	}
	public static class EventSourceExtension
    {
		

		public static async Task Emit<TPayload>(this IEventEmitService Service,  TPayload Payload)
		{
			var topic = TypeTopic<TPayload>.Name;
			var e = await Service.Create(topic, Payload);
			await e.Commit();
		}
		public static async Task Emit<TPayload>(this IEventEmitService Service,string Topic,TPayload Payload)
		{
			var e = await Service.Create(Topic,Payload);
			await e.Commit();
		}
		
		
		public static IDisposable Subscribe<TPayload>(
			this IEventSubscribeService Resolver, 
			string TopicFilter, 
			Func<IEvent<TPayload>, Task> Callback, 
			EventDeliveryPolicy Policy = EventDeliveryPolicy.NoGuarantee
			)
		{
			return Resolver.Subscribe(TopicFilter, null, Policy, new DelegateEventObserver<TPayload>(Callback));
		}
		
		public static IDisposable Filter<TSrcPayload,TDstPayload>(
			this IEventSubscribeService Resolver,
			string TopicFilter,
			IEventEmitService EmitService,
			Func<IEvent<TSrcPayload>, Task<(string Topic,TDstPayload Payload)>> Filter
			)
			where TSrcPayload: class, IEvent
			where TDstPayload: class, IEvent
		{
			return Resolver.Subscribe<TSrcPayload>(
                TopicFilter,
                async se =>
                {
					var (dstTopic, dstPayload) = await Filter(se);
					if (dstTopic != null)
					{
						var de = await EmitService.Create(dstTopic, dstPayload);
						await de.Commit();
					}
                }
                );
		}

	}

}
