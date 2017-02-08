using System.Collections.Generic;

namespace SF.Services.ManagedServices.Runtime
{
	public interface IServiceStartup<T>
	{
		void Startup();
	}
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
