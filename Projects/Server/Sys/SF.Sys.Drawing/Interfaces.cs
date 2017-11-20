#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Drawing
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
