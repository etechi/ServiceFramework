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
using System.Threading.Tasks;

namespace SF.Sys.Events
{
	public class DelegateEventObserver<TPayload> : IEventObserver<TPayload>
	{
		public Func<IEvent<TPayload>, Task<object>> DelegatePrepare { get; }
		public Func<IEvent<TPayload>, object, Task> DelegateCommit { get; }
		public Func<IEvent<TPayload>, object, Exception,Task> DelegateCancel { get; }
		public DelegateEventObserver(
			Func<IEvent<TPayload>, Task<object>> DelegatePrepare,
			Func<IEvent<TPayload>, object, Task> DelegateCommit,
			Func<IEvent<TPayload>, object, Exception,Task> DelegateCancel
			){
			this.DelegateCancel = DelegateCancel;
			this.DelegateCommit = DelegateCommit;
			this.DelegatePrepare = DelegatePrepare;
		}
		public DelegateEventObserver(
			Func<IEvent<TPayload>, Task> DelegateCommit
			):this(null,(i,c)=>DelegateCommit(i),null)
		{
		}

		public Task Cancel(IEvent<TPayload> EventInstance, object Context,Exception Exception)
		{
			return DelegateCancel?.Invoke(EventInstance, Context, Exception) ?? Task.CompletedTask;
		}

		public Task Commit(IEvent<TPayload> EventInstance, object Context)
		{
			return DelegateCommit?.Invoke(EventInstance, Context) ?? Task.CompletedTask;
		}

		static Task<object> NullTask { get; } = Task.FromResult<object>(null);
		public Task<object> Prepare(IEvent<TPayload> EventInstance)
		{
			return DelegatePrepare?.Invoke(EventInstance) ?? NullTask;
		}
	}
}
