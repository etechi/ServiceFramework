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
using System.Threading;

namespace SF.Sys.Events
{	
	public static class TaskEventService
	{
		public static Task AddEventHandler<TPayload>(
			this IEventSubscribeService EventSubscribeService,
			Func<IEvent<TPayload>, Task> EventCallback,
			EventDeliveryPolicy Policy= EventDeliveryPolicy.NoGuarantee,
			CancellationToken token = default(CancellationToken)
			)
		{
			var topic = TypeTopic<TPayload>.Name;
			return AddEventHandler<TPayload>(EventSubscribeService, topic, EventCallback, Policy ,token);
		}

		public static async Task AddEventHandler<TPayload>(
            this IEventSubscribeService EventSubscribeService,
            string EventTopicFilter,
            Func<IEvent<TPayload>, Task> EventCallback,
			EventDeliveryPolicy Policy = EventDeliveryPolicy.NoGuarantee,
			CancellationToken token= default(CancellationToken)
			) 
		{
			var tcs = new TaskCompletionSource<int>();
			using (token.Register(() => tcs.TrySetResult(0)))
				using (EventSubscribeService
					.Subscribe<TPayload>(
					EventTopicFilter,
					e =>
					{
						if (token.IsCancellationRequested)
							return Task.CompletedTask;
						return EventCallback(e);
					},
					Policy
					))
				{
					await tcs.Task;
				}

		}
       

    }
	
}
