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

using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SF.Services.Tests
{
	public class TestResult
	{
		public static TestResult Success { get; } = new TestResult("测试成功",null);
		public string Message { get; }
		public string Source { get; }
		public override string ToString()
		{
			return Source==null?Message: Source+":"+ Message;
		}
		public TestResult(string Source,string Message)
		{
			this.Message = Message;
			this.Source = Source;
		}
		public static TestResult Merge(IEnumerable<TestResult> results)
		{
			var rs = results.ToArray();
			var es = rs.Where(r => r != TestResult.Success).ToArray();
			if (es.Length == 0) return TestResult.Success;
			if (es.Length == 1) return es[0];
			return new GroupTestResult(null,es.ToArray());
		}

	}
	public class GroupTestResult : TestResult
	{
		public TestResult[] Results { get; }
		public override string ToString()
		{
			return base.ToString() + "::" + string.Join(",", (object[])Results);
		}
		public GroupTestResult(string Source,TestResult[] Results,string Message=null):
			base(Source,Message??"多项测试失败")
		{
			this.Results = Results;
		}
	}
	public class AssertResult<T> : TestResult
	{
		public T ExpectValue { get; }
		public T TestValue { get; }
		public AssertResult(string Source,T expectValue,T testValue)
			:base(Source,$"断言失败:期望值:{expectValue} 实际值:{testValue}")
		{
			this.ExpectValue = expectValue;
			this.TestValue = testValue;
		}
	}
	
	public interface IValueAssert<T>
	{
		TestResult Assert(string Source,T ExpectValue,T TestValue);
	}
	public interface IValueAssertProvider
	{
		IValueAssert<T> GetValueAssert<T>(PropertyInfo Prop);
	}
}
