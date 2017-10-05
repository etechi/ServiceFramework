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
    public interface IValueTestHelper<T>:
		IValueValidator<T>,
		IValueSampleGenerator<T>,
		IValueAssert<T>
    {
	}
	public interface IValueTestHelperCache 
	{
		IValueTestHelper<T> GetHelper<T>(PropertyInfo Prop);
	}
}
