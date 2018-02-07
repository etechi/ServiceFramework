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
	public class EventSubscriber<TPayload> : IEventSubscriber<TPayload>,IDisposable 
	{
		IDisposable _Disposable;
		IEventSubscribeService SubscribeService { get; }
		object EventHandler;
		public EventSubscriber(
			IEventSubscribeService SubscribeService
			)
		{
			this.SubscribeService = SubscribeService;
		}

		public void Dispose()
		{
			Disposable.Release(ref _Disposable);
		}

		public void Wait(
			Func<IEvent<TPayload>, Task> Callback,
			EventDeliveryPolicy Policy=EventDeliveryPolicy.NoGuarantee,
			string EventTopic=null,
			string SubscriberIdent = null
			)
		{
			if (_Disposable != null)
				throw new InvalidOperationException();

			if (EventTopic == null)
			{
				var et = typeof(TPayload);
				EventTopic = et.Namespace+"/"+ et.Name;
			}
			_Disposable=SubscribeService.Subscribe(EventTopic,
				SubscriberIdent,
				Policy,
				new DelegateEventObserver<TPayload>(
				 Callback
				 )
				);
		}
	}

}
