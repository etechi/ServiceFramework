using SF.Metadata;
using SF.Metadata.Models;
using System.IO;
using System.Reflection;
namespace SF.Core.NetworkService
{
	public interface IHttpRequestSource
	{
		System.Net.Http.HttpRequestMessage Request{ get; }
	}
}
