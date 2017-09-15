using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Media
{
	public class MediaMeta:IMediaMeta
	{
		public string Type { get; set; }
		public string Id { get; set; }
		public string Mime { get; set; }
		public string Name { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Length { get; set; }
	}
}

