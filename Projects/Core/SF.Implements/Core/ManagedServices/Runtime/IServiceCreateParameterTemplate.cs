
namespace SF.Core.ManagedServices.Runtime
{
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		string GetServiceIdent(string Path);
		ManagedServices.IServiceInstanceIdent GetServiceInstanceIdent();
	}
}
