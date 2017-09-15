using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FileProviders
{ 
    public interface IFileInfo
    {
		bool Exists{get;}

		long Length{get;}

		string PhysicalPath{get;}

		string Name{get;}

		DateTimeOffset LastModified{get;}

		bool IsDirectory{get;}

		Stream CreateReadStream();
	}
	
}
