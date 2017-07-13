
using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	interface IServiceInstanceSetting
	{
		long? InstanceId { get; }
		Type ServiceType { get; }
	}
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		IServiceInstanceSetting GetServiceIdent(string Path);
	}
}
