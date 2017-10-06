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
		public static TestResult Success { get; } = new TestResult("测试成功");
		public string Message { get; }
		public TestResult(string Message)
		{
			this.Message = Message;
		}
		public static TestResult Merge(IEnumerable<TestResult> results)
		{
			var rs = results.ToArray();
			var es = rs.Where(r => r != TestResult.Success).ToArray();
			if (es.Length == 0) return TestResult.Success;
			if (es.Length == 1) return es[0];
			return new GroupTestResult(es.ToArray());
		}

	}
	public class GroupTestResult : TestResult
	{
		public TestResult[] Results { get; }
		public GroupTestResult(TestResult[] Results,string Message=null):
			base(Message??"多项测试失败")
		{
			this.Results = Results;
		}
	}
	public class AssertResult<T> : TestResult
	{
		public T ExpectValue { get; }
		public T TestValue { get; }
		public AssertResult(T expectValue,T testValue)
			:base($"断言失败:期望值:{expectValue} 实际值:{testValue}")
		{
			this.ExpectValue = expectValue;
			this.TestValue = testValue;
		}
	}
	
	public interface IValueAssert<T>
	{
		TestResult Assert(T ExpectValue,T TestValue);
	}
	public interface IValueAssertProvider
	{
		IValueAssert<T> GetValueAssert<T>(PropertyInfo Prop);
	}
}
