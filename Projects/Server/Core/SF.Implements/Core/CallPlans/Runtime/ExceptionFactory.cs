using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;

namespace SF.Core.CallPlans.Runtime
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
