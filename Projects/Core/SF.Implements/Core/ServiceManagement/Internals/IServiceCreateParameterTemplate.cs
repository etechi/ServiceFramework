
using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	interface IServiceInstanceSetting
	{
		string InstanceId { get; }
		string ServiceType { get; }
	}
	interface IServiceCreateParameterTemplate
	{
		int AppId { get; }
		object GetArgument(int Index);
		IServiceInstanceSetting GetServiceIdent(string Path);
		//IServiceInterfaceMeta GetServiceInstanceIdent();
	}
}
