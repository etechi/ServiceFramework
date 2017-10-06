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
	public interface ITestContext
	{
		ITestAssert Assert { get; }
		ISampleSeed SampleSeed { get; }
	}
	public interface ITestCase
	{
		string Category { get; }
		string Name { get; }
	}
	public interface ITestCaseProvider
	{
		IEnumerable<ITestCase> GetTestCases();
		Task Execute(ITestCase Case);
	}
}
