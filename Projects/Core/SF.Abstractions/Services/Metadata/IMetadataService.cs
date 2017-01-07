using SF.Annotations;
using SF.Metadata.Models;
using System.Reflection;
namespace SF.Services.Metadata
{
	[NetworkService]
	public interface IServiceMetadataService
	{
		//[Authorize(Roles ="admin")]
		Models.Library Json();
		string Typescript(bool all = true);
	}
}
