using SF.Metadata;
using SF.Metadata.Models;
using System.IO;
using System.Reflection;
namespace SF.Services.NetworkService
{
	public interface IHttpRequestSource
	{
		System.Net.Http.HttpRequestMessage Request{ get; }
	}
}
