using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Drawing
{
    public interface IImageProvider
    {
		IImage Load(Stream Stream);
		IImageBuffer NewImageBuffer(SF.Maths.Size Size);
		IDrawContext NewDrawContext(IImageBuffer Size);
	}
}
