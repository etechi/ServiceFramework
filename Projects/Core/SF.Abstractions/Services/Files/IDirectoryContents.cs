using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.FileProviders
{ 
	public interface IDirectoryContents : IEnumerable<IFileInfo>, IEnumerable
	{
		bool Exists{get;}
	}
}
