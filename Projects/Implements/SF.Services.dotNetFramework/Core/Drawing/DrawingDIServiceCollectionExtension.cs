﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Maths;
using SF.Core.Drawing;

namespace SF.Core.ServiceManagement
{
	public static class DrawingDIServiceCollectionExtension
	{
		public static IServiceCollection UseSystemDrawing(this IServiceCollection sc)
		{
			sc.AddSingleton<IImageProvider, Drawing.dotNetFramework.ImageProcessor>();
			return sc;
		}
	}
}
