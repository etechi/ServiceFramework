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
using System.Threading.Tasks;

namespace SF.Clients
{
	
	public class LocalClientService : IClientService, IAccessSource
	{
		public IAccessSource AccessSource => this;

		public long? CurrentScopeId { get; set; }

		IReadOnlyDictionary<string, string> IAccessSource.ExtraValues { get; } = new Dictionary<string, string>();

		string _ClientAddress = "local";
		string _ClientAgent = "console";
		ClientDeviceType _ClientDeviceType = ClientDeviceType.Console;

		string IAccessSource.ClientAddress => _ClientAddress;

		string IAccessSource.ClientAgent => _ClientAgent;

		ClientDeviceType IAccessSource.DeviceType => _ClientDeviceType;

		string _AccessToken;
		public string GetAccessToken()
		{
			return _AccessToken;
		}

		public Task SetAccessToken(string AccessToken)
		{
			_AccessToken = AccessToken;
			return Task.CompletedTask;
		}
	}
}
