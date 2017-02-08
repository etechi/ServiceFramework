
namespace SF.Services.ManagedServices.Runtime
{
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		string GetServiceIdent(string Path);
		ManagedService.IServiceInstanceIdent GetServiceInstanceIdent();
	}
}
