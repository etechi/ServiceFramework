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

using SF.Core.ServiceManagement.Management;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.AsyncTrades
{
	public interface IAsyncTradeRequest
	{
		string CustomerIdent { get; }
		string CustomerTradeIdent { get; }
	}
	public enum AsyncTradeTarget
	{
		Successful,
		Cancelled
	}
	public enum AsyncTradeState
	{
		Success,
		Cancelled,
		Pending
	}

	public interface IAsyncTradeState<TResult>
	{
		string Id { get; }
		AsyncTradeTarget Target { get; }
		AsyncTradeState State { get; }
		TResult Result { get; }
	}
	public interface IAsyncTradeNotifyHandler<TResult>
	{
		Task Handle(IAsyncTradeState<TResult> State);
	}

	public interface IAsyncTradeService<TRequest, TResult> where TRequest : IAsyncTradeRequest
	{
		Task<string> Create(TRequest Request);
		Task<IAsyncTradeState<TResult>> Query(string CustomerIdent,string CustomerTradeId);
		Task<IAsyncTradeState<TResult>> Push(string CustomerIdent,string CustomerTradeId, object Context);
		Task<IAsyncTradeState<TResult>> Cancel(string CustomerIdent, string CustomerTradeId);
	}

}
