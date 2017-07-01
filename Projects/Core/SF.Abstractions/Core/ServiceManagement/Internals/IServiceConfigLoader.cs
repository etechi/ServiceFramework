using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceStartup<T>
	{
		void Startup();
	}

	public interface IServiceInterfaceConfig
	{
		string ImplementType { get; }
		string Setting { get; }
	}
	public interface IServiceConfig
	{
		string ServiceType { get; }
		long Id { get; }
		int AppId { get; }
		IReadOnlyDictionary<string, IServiceInterfaceConfig> Settings { get; }
	}
	[UnmanagedService]
	public interface IServiceConfigLoader
	{
		IServiceConfig GetConfig(string ServiceType,int AppId, long Id);
	}
	
}
