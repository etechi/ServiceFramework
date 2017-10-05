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
	public class AssertResult
	{
		public static AssertResult Success { get; } = new AssertResult();
		
	}
	public class AssertResult<T> : AssertResult
	{
		public T ExpectValue { get; }
		public T TestValue { get; }
		public AssertResult(T expectValue,T testValue)
		{
			this.ExpectValue = expectValue;
			this.TestValue = testValue;
		}
	}
	public class GroupAssertResult : AssertResult
	{
		public AssertResult[] Results { get; }
		public GroupAssertResult(AssertResult[] Results)
		{
			this.Results = Results;
		}
	}
	public interface IValueAssert<T>
	{
		AssertResult Assert(T ExpectValue,T TestValue);
	}
	public interface IValueAssertProvider
	{
		IValueAssert<T> GetValueAssert<T>(PropertyInfo Prop);
	}
}
