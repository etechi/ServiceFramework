using System.Collections.Generic;

namespace SF.ServiceManagement
{
	public interface IServiceConfig
	{
		string ServiceType { get; }
		string ImplementType { get; }
		string CreateArguments { get; }
	}
	public interface IServiceConfigLoader
	{
		IServiceConfig GetConfig(string Id);
	}
}
