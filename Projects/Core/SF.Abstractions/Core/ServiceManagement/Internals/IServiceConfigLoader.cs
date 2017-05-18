using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceStartup<T>
	{
		void Startup();
	}

	public interface IServiceConfig
	{
		string ServiceType { get; }
		string ImplementType { get; }
		long Id { get; }
		string Settings { get; }
	}
	[UnmanagedService]
	public interface IServiceConfigLoader
	{
		IServiceConfig GetConfig(string ServiceType,long Id);
	}
	
}
