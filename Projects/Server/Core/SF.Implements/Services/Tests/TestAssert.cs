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
