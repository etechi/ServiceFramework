using SF.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public static class ObjectExtension
	{
		public static bool IsDefault<T>(this T value)
		{
			return EqualityComparer<T>.Default.Equals(value, default(T));
		}
	}
}
