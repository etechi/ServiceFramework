using System;
using Xunit;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using SF.Core.ServiceManagement;
using SF.Core.DI;
using SF.Core.ServiceManagement.Storages;
using SF.Core.CallPlans;
using System.Threading.Tasks;
using SF.Entities;
using SF.Core;

namespace SF.UT.Core
{
	public class CallPlans
	{
		static Func<T, int, int> Register<T>(T ThisObject,Expression<Func<T,int, int>> expr)
			where T:class
		{
			//var arg = Expression.Parameter(typeof(T));
			//var body = expr.Body.Replace(e =>
			//{
			//	var c = e as ConstantExpression;
			//	if (c == null) return e;
			//	if (c.Value == ThisObject)
			//		return arg;
			//	if (c.Type.IsSystemType())
			//		return c;
			//	throw new NotSupportedException($"不支持引用this以外的对象，{c.Type} {c.Value}");
			//});
			var func = expr.Compile();
			return func;
		}
		public class Model
		{

		}
		public class ThisObject
		{
			public int A { get; } = 10;
		}
		[Fact]
		public void ExprTest()
		{
			var obj = new ThisObject();
			var func=Register(
				obj,
				(o,ctx) =>
					o.A+ctx					
				);
			var re = func(obj, 20);
			Assert.Equal(30, re);
		}
	}
}
