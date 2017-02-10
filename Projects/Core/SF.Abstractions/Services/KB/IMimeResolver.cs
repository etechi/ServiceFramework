using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.KB.Mime
{
    public interface IMimeResolver
    {
		string FileExtensionToMime(string extension);
		string MimeToFileExtension(string mime);
	}
}
