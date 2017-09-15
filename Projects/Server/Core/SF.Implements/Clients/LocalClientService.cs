
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
