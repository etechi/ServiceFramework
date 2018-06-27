using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SF.Sys.UnitTest;
using System.Linq.Expressions;
using SF.Utils.Parsers;

namespace SF.UT.系统
{
	[TestClass]
	public class ParserTest : TestBase
	{
		
		[TestMethod]
		public void TestParse()
		{
			var ctxArg = Expression.Parameter(typeof(object), "arg");
			var ctx = new ParserBuildContext
			{
				BuildMemberExpression = (e, n) => Expression.Property(e, n),
				BuildVarExpression = (n) => n == "this" ? ctxArg : throw new ArgumentException()
			};
			var p = Parser.Expr(ctx);
			var re = p(Content.From("1+2"));
			Assert.IsNotNull(re);
		}
		
	}

}
