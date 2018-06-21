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
	public interface IEvent
	{
		long Id { get; }
		string Topic { get; }
	}
	public interface IEvent<TPayload> : IEvent
	{
		TPayload Payload { get; }
	}

	public class CommonEvent<TPayload> : IEvent<TPayload>
	{
		public long Id { get; set; }
		public string Topic { get; set; }
		public TPayload Payload { get; set; }
	}

	
	public interface IEventEmitter 
	{
		long Id { get; }
		string Topic { get; }
		Task Cancel(Exception Exception);
		Task Commit();
	}
	public interface IEventEmitter<TPayload> : IEventEmitter {
		TPayload Payload { get; }
	}


	public interface IEventValidator<T>
	{
		Task<bool> Validate(T Event);
	}

	public interface IEventSubscriber<TPayload> 
	{
		IDisposable Wait(
			Func<IEvent<TPayload>,Task> Callback,
			EventDeliveryPolicy Policy = EventDeliveryPolicy.NoGuarantee,
			string Topic = null,
			string SubscriberIdent = null
			);
	}
	public interface IEventObserver<TPayload> 
	{
		Task<object> Prepare(IEvent<TPayload> Event);
		Task Commit(IEvent<TPayload> Event,object Context);
		Task Cancel(IEvent<TPayload> Event, object Context, Exception Exception);
	}
	

	public interface IEventSubscribeService
	{
		IDisposable Subscribe<TPayload>(
			string TopicFilter,
			string SubscriberIdent,
			EventDeliveryPolicy Policy,
			IEventObserver<TPayload> Observer
			);
	}


	public interface IEventEmitService
	{
		Task<IEventEmitter> Create<TPayload>(string Topic, TPayload Payload);
	}
   
}
