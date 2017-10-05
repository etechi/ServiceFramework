using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SF.Services.Tests
{
	
	public class TestAssert : ITestAssert
	{
		public void AssertFailed<T>(T expect, T test)
		{
			throw new InvalidOperationException($"断言错误，期望值{expect},实际值{test}");
		}

		public void ValidateFailed<T>(T value, string message)
		{
			throw new InvalidOperationException($"{value}验证错误，{message}");
		}
	}
}
