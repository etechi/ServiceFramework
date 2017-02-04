using SF.Metadata;
using SF.Metadata.Models;
using System.Net.Http;
using System.Reflection;
namespace SF.Services.NetworkService
{
	[NetworkService]
	public interface IServiceMetadataService
	{
		//[Authorize(Roles ="admin")]
		Metadata.Library Json();
		StringContent Typescript(bool all = true);
	}
}
