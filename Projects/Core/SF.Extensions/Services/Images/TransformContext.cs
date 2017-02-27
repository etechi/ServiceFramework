using SF.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.Drawing
{
	
	class TransformContext :ITransformContext, IDisposable
	{
		IImageBuffer _buffer1;
		IImageBuffer _buffer2;
		int _level;
		Stack<ExecuteFlag> _flags = new Stack<ExecuteFlag>();
		IImageProvider Provider { get; }
		public TransformContext(IImageProvider Provider)
		{
			this.Provider = Provider;
		}
		public void Dispose()
		{
			if (_buffer1 != null)
				_buffer1.Dispose();
			if (_buffer2 != null)
				_buffer2.Dispose();
		}
		public T Execute<T>(Transform<T> Transform, ImageContext src,ExecuteFlag flags= ExecuteFlag.None)
		{
			_level++;
			_flags.Push(flags);
			var re = Transform(src, this);
			_flags.Pop();
			_level--;
			return re;
		}
		public ExecuteFlag Flags {
			get { return _flags.Peek(); }
		}
		public IImageBuffer GetTarget(Size Size,bool force=false)
		{
			if (!force)
				force = (Flags & ExecuteFlag.StrictSize) == ExecuteFlag.StrictSize;

			var t = _buffer2;
			_buffer2 = _buffer1;
			_buffer1 = t;
			if(_buffer1==null || 
				force && (_buffer1.Size.Width!=Size.Width || _buffer1.Size.Height!=Size.Height) ||
				!force && (_buffer1.Size.Width<Size.Width || _buffer1.Size.Height<Size.Height)
				)
			{
				if (_buffer1 != null)
					_buffer1.Dispose();
				_buffer1 = Provider.NewImageBuffer(Size);
			}
			return _buffer1;
		}

		public IDrawContext NewGraphics(IImageBuffer image)
		{
			var g=Provider.NewDrawContext(image);
			return g;
		}
	}

	
}
