using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	public interface IMediaMeta
    {
		string Type { get; }
		string Id { get; }
		string Mime { get; }
		string Name { get; }
		int Width { get; }
		int Height { get; }
		int Length { get; }
	}
	
}

