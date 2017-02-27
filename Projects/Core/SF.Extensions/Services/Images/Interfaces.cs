using SF.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing
{
	
	[Flags]
	public enum ExecuteFlag
	{
		None=0,
		StrictSize=1
	}
	public interface ITransformContext
	{
		IImageBuffer GetTarget(Size Size, bool force=false);
		IDrawContext NewGraphics(IImageBuffer image);
		ExecuteFlag Flags { get; }
		T Execute<T>(Transform<T> Transform, ImageContext src, ExecuteFlag flags= ExecuteFlag.None);
	}
	public class ImageMemoryBuffer
	{
		public byte[] Data { get; set; }
		public string Mime { get; set; }
		public Size Size { get; set; }
	}
	public class ImageContext
	{
		public IImage Image { get; set; }
		public Size Size { get; set; }
	}

	public delegate T Transform<T>(ImageContext src, ITransformContext ctx);

}
