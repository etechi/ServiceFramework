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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace SF.Sys.Plans.Runtime
{
	static class ExceptionFactory
	{
		static System.Collections.Concurrent.ConcurrentDictionary<string, Func<string, Exception>> factories = new System.Collections.Concurrent.ConcurrentDictionary<string, Func<string, Exception>>();
		static Func<string,Exception> GetCreator(string Type)
		{
			Func<string, Exception> func;
			if (factories.TryGetValue(Type, out func))
				return func;

			var type = System.Type.GetType(Type, false);
			if (type == null)
				return null;
			var constructor=type.GetConstructor(new[] { typeof(string) });
			var p = Expression.Parameter(typeof(string), "msg");
			
			func=Expression.Lambda<Func<string, Exception>>(
				Expression.Convert(
					Expression.New(constructor, p),
					typeof(Exception)
					),
				p).Compile();
			return factories.GetOrAdd(Type, func);
		}
		public static bool Exists(string Type)
		{
			return GetCreator(Type) != null;
		}
		public static Exception Create(string Type,string Message)
		{
			var func = GetCreator(Type);
			if (func == null)
				throw new ArgumentException("找不到异常类型：" + Type);
			return func(Message);
		}
		public static void VerifyErrorType(Type Type)
		{
			var func = GetCreator(Type.FullName);
			if (func == null)
				throw new ArgumentException("创建异常构造器失败，异常类型：" + Type);
		}
		public static Exception CreateFromError(string Error)
		{
			if (string.IsNullOrEmpty(Error))
				return null;
			var i = Error.IndexOf(':');
			if (i == -1) throw new ArgumentException("错误字符串格式错误：" + Error);
			var Type = Error.Substring(0, i);
			var Msg = Error.Substring(i + 1);
			return Create(Type, Msg);
		}
	}
}
