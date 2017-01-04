
namespace SF.ServiceManagement.Internal
{
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		string GetServiceIdent(string Path);
	}
}
