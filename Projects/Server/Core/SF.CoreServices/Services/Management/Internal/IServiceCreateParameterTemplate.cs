
namespace SF.Services.Management.Internal
{
	interface IServiceCreateParameterTemplate
	{
		object GetArgument(int Index);
		string GetServiceIdent(string Path);
	}
}
