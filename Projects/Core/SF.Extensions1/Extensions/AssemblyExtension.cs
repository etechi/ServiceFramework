using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF
{
	public static class AssemblyExtension
	{
		public static string ReadResourceString(this Assembly ass, string path)
		{
			using (var s = ass.GetManifestResourceStream(ass.GetName().Name + "." + path.Replace("/", ".")))
			{
				var data = s.ReadToEnd();
				return data.UTF8String();
			}
		}
	}
}
