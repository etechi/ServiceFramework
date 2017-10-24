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
using System.Security.Claims;
using System.Threading.Tasks;

namespace SF.Clients
{
	
	public class LocalClientService : IClientService, IUserAgent,IAccessToken
	{
		public IUserAgent UserAgent => this;

		public long? CurrentScopeId { get; set; }

		IReadOnlyDictionary<string, string> IUserAgent.ExtraValues { get; } = new Dictionary<string, string>();

		string _ClientAddress = "local";
		string _ClientAgent = "console";
		ClientDeviceType _ClientDeviceType = ClientDeviceType.Console;

		string IUserAgent.Address => _ClientAddress;

		string IUserAgent.AgentName => _ClientAgent;

		ClientDeviceType IUserAgent.DeviceType => _ClientDeviceType;


		ClaimsPrincipal _User;
		public ClaimsPrincipal User => _User;

		Stack<ClaimsPrincipal> _OperatorStack;
		public ClaimsPrincipal Operator =>
			(_OperatorStack?.Count ?? 0) == 0 ? User : _OperatorStack.Peek();

		
		public Task SignInAsync(ClaimsPrincipal User)
		{
			_User = User;
			return Task.CompletedTask;
		}

		public IDisposable UseOperator(ClaimsPrincipal NewOperator)
		{
			if (_OperatorStack == null)
				_OperatorStack = new Stack<ClaimsPrincipal>();
			_OperatorStack.Push(NewOperator);
			return Disposable.FromAction(() => _OperatorStack.Pop());
		}
	}
}
