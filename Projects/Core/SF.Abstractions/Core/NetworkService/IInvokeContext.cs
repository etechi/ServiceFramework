using SF.Metadata;
using SF.Metadata.Models;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Core.NetworkService
{
	[UnmanagedService]
	public interface IInvokeRequest
	{
		string Method { get; }
		string Uri { get; }
		IReadOnlyDictionary<string,IEnumerable<string>> Headers { get; }
	}
	[UnmanagedService]
	public interface IInvokeResponse
	{
		string Status { get; set; }
		IDictionary<string, IEnumerable<string>> Headers { get; }
	}
	[UnmanagedService]
	public interface IInvokeContext
	{
		IInvokeRequest Request { get; }
		IInvokeResponse Response { get; }
	}
}
