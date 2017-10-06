using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Tests
{
    public interface ITestAssert
	{
		void ValidateFailed<T>(T value,string message);
		void AssertFailed<T>(T expect, T test,string message=null);
		void Equal<T>(T expect, T test);
		void NotEqual<T>(T expect, T test);
		void Success(TestResult Result);
	}
}
