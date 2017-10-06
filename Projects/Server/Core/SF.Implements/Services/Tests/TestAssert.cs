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

		public void AssertFailed<T>(T expect, T test,string Message=null)
		{
			throw new InvalidOperationException($"断言错误:{Message},期望值{expect},实际值{test}");
		}

		public void Equal<T>(T expect, T test)
		{
			if (Poco.DeepEquals(expect, test))
				return;
			AssertFailed(expect, test,"实际值和期望值不同");
		}
		public void NotEqual<T>(T expect, T test)
		{
			if (Poco.DeepEquals(expect, test))
				AssertFailed(expect, test, "实际值和期望值相同");
		}
		public void ValidateFailed<T>(T value, string message)
		{
			throw new InvalidOperationException($"{value}验证错误，{message}");
		}
		public void Success(TestResult Result)
		{
			if (Result == TestResult.Success)
				return;
			throw new InvalidOperationException($"测试失败:" + Result.Message);
		}
	}
}
