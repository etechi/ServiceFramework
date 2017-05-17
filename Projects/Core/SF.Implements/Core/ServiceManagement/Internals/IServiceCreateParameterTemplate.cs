
using System.Collections.Generic;

namespace SF.Core.ServiceManagement.Internals
{
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		KeyValuePair<string,long> GetServiceIdent(string Path);
		//IServiceInterfaceMeta GetServiceInstanceIdent();
	}
}
