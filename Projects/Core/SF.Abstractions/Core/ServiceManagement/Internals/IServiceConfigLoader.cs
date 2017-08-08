﻿using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceStartup<T>
	{
		void Startup();
	}

	public interface IServiceConfig
	{
		long Id { get; }
		string Name { get; }
		long? ParentId { get; }
		string ServiceType { get; }
		string ImplementType { get; }
		string Setting { get; }
	}
	public interface IServiceConfigLoader
	{
		IServiceConfig GetConfig(long Id);
	}
	
}
