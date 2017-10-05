using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using SF.Services.Tests;
using SF.Services.Tests.ValueAsserts;
using SF.Services.Tests.SampleGenerators;

namespace SF.Core.ServiceManagement
{
	public static class TestDIExtension
	{
		public static IServiceCollection AddTestServices(this IServiceCollection sc)
		{
			sc.AddSingleton<IValueTestHelperCache, ValueTestHelperCache>();
			sc.AddSingleton<IValueAssertProvider, DefaultValueAssertProvider>();
			sc.AddSingleton<IValueValidatorProvider, DefaultValueValidatorProvider>();
			sc.AddSingleton<IValueSampleGeneratorProvider, DefaultSampleGeneratorProvider>();

			sc.AddSingleton<ITestAssert, TestAssert>();
			sc.AddSingleton<ISampleSeed, SampleSeed>();
			return sc;
		}
	}
}
