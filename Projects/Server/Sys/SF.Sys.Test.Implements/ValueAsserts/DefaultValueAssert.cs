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


using System.Collections.Generic;
using System.Reflection;

namespace SF.Sys.Tests.ValueAsserts
{

	class DefaultValueAssertProvider : IValueAssertProvider
	{
		ITestAssert TestAssert { get; }
		class DefaultValueAssert<T> : IValueAssert<T>
		{
			ITestAssert TestAssert { get; }
			public DefaultValueAssert(ITestAssert TestAssert)
			{
				this.TestAssert = TestAssert;
			}
			public TestResult Assert(string Source,T ExpectValue, T TestValue)
			{
				if (Comparer<T>.Default.Compare(ExpectValue, TestValue) == 0)
					return TestResult.Success;
				else
					return new AssertResult<T>(Source,ExpectValue, TestValue);

			}
		}
		public IValueAssert<T> GetValueAssert<T>(PropertyInfo Prop)
		{
			return new DefaultValueAssert<T>(TestAssert);
		}
	}
}
