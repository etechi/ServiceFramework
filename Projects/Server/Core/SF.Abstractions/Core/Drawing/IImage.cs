using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing
{
	public class ExifProperty
	{
		public int Id { get; set; }
		public int Len { get; set; }
		public short Type { get; set; }
		public byte[] Value { get; set; }
	}
	public interface IImage : IDisposable
    {
		Maths.Size Size { get; }
		string Format { get; }
		IEnumerable<ExifProperty> ExifProperties { get; }
		void SaveTo(Stream Stream, string Format, int OutputQuality);
	}
}
