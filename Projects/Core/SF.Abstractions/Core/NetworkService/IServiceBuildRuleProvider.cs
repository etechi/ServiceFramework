using SF.Metadata.Models;
using System.Collections.Generic;
using System.Reflection;
namespace SF.Core.NetworkService
{
	public interface IServiceBuildRuleProvider
	{
		IEnumerable<MethodInfo> GetServiceMethods(System.Type type);
		IEnumerable<ParameterInfo> GetMethodParameters(System.Reflection.MethodInfo method);
		string FormatMethodName(MethodInfo method);
		string FormatServiceName(System.Type type);
		ParameterInfo DetectHeavyParameter(MethodInfo method); 
	}
}
