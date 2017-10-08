using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using SF.ADT;

namespace SF.Services.Tests
{
	
	public class TestAssert : ITestAssert
	{
		public TestAssert()
		{
		}

		public void AssertFailed<T>(string Message,T expect, T test)
		{
			throw new InvalidOperationException($"断言错误:{Message},期望值{expect},实际值{test}");
		}

		public void Equal<T>(string Message,T expect, T test)
		{
			if (Poco.DeepEquals(expect, test))
				return;
			AssertFailed(Message + ",实际值与期望值不同",expect, test);
		}
		public void NotEqual<T>(string Message, T expect, T test)
		{
			if (Poco.DeepEquals(expect, test))
				AssertFailed(Message + ",实际值与期望值相同",expect, test);
		}
		public void ValidateFailed<T>(string message, T value )
		{
			throw new InvalidOperationException($"{value}验证错误，{message}");
		}
		public void Success(string Message,TestResult Result)
		{
			if (Result == TestResult.Success)
				return;
			throw new InvalidOperationException($"测试失败:{Message} {Result}");
		}
	}
}
