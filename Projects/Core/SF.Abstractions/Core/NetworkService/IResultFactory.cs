using SF.Metadata;
using SF.Metadata.Models;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using SF.Core.ServiceManagement;

namespace SF.Core.NetworkService
{
	public interface IResultFactory
	{
		object Json<T>(T obj);
		object Content(string Text, string Mime, System.Text.Encoding Encoding, string FileName);
		object Content(byte[] Data, string Mime, System.Text.Encoding Encoding,string FileName);
		object File(string Path, string Mime, string FileName=null);
		object Redirect(string Path);
		object LocalRedirect(string Path);
	}
}
