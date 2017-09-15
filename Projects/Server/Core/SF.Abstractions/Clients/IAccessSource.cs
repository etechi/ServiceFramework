using System.Collections.Generic;

namespace SF.Clients
{
	
	public interface IAccessSource
	{
		IReadOnlyDictionary<string,string> ExtraValues { get; }
		string ClientAddress { get;  }
		string ClientAgent { get; }
		ClientDeviceType DeviceType { get; }
	}
}
