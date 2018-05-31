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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys.IO
{
	public static class FS
	{
		public static void EnsureDirectory(string Path)
		{
			Directory.CreateDirectory(Path);
		}
		public static async Task UseWriteStream(string Path, Func<Stream, Task> Callback)
		{
			EnsureDirectory(System.IO.Path.GetDirectoryName(Path));
			using (var s = new FileStream(Path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read))
				await Callback(s);
		}
		public static Task UseTextWriter(string Path,Func<TextWriter,Task> Callback,Encoding encoding=null)
		{
			return UseWriteStream(Path,async s =>
			{
				using (var w = new StreamWriter(s, encoding ?? Encoding.UTF8))
					await Callback(w);
			});
		}
	}
}
