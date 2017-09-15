using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FileProviders
{ 
    
	public interface IFileProvider
	{
		IFileInfo GetFileInfo(string subpath);

		IDirectoryContents GetDirectoryContents(string subpath);

		IDisposable Watch(string filter, Action Callback);
	}
}
