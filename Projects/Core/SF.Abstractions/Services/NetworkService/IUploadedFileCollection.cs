using SF.Metadata;
using SF.Metadata.Models;
using System.IO;
using System.Reflection;
namespace SF.Services.NetworkService
{
	public interface IUploadedFile
	{
		string FileName { get; }
		Stream InputStream { get; }
		long ContentLength { get; }
		string ContentType { get; set; }
	}
	public interface IUploadedFileCollection
	{
		IUploadedFile[] Files { get; }
	}
}
