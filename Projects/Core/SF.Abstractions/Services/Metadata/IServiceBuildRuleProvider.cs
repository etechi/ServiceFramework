using SF.Metadata.Models;
using System.Reflection;
namespace SF.Services.Metadata
{
	public interface IServiceBuildRuleProvider
	{
		string FormatServiceName(System.Type type);
		ParameterInfo DetectHeavyParameter(MethodInfo method); 
	}
}
