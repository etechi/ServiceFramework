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
		void ValidateFailed<T>(string Message, T value);
		void AssertFailed<T>(string Message, T expect, T test);
		void Equal<T>(string Message,T expect, T test);
		void NotEqual<T>(string Message, T expect, T test);
		void Success(string Message, TestResult Result);
	}
}
