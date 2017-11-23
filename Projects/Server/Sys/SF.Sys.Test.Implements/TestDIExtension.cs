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
using SF.Sys.Tests;
using SF.Sys.Tests.ValueAsserts;
using SF.Sys.Tests.SampleGenerators;
using SF.Sys.Tests.NumberValueTypes;

namespace SF.Sys.Services.Management
{
	public static class TestDIExtension
	{
		public static IServiceCollection AddTestServices(this IServiceCollection sc)
		{
			sc.AddSingleton<INumberValueTypeProvider, NumberValueTypeProvider>();
			sc.AddSingleton<IValueTestHelperCache, ValueTestHelperCache>();
			sc.AddSingleton<IValueAssertProvider, DefaultValueAssertProvider>();
			sc.AddSingleton<IValueValidatorProvider, DefaultValueValidatorProvider>();
			sc.AddSingleton<IValueSampleGeneratorProvider, DefaultSampleGeneratorProvider>();

			sc.AddSingleton<ITestAssert, TestAssert>();
			sc.AddSingleton<ISampleSeed, SampleSeed>();

			sc.AddInitializer("test", "TestProviders", async sp =>
			{
				var providers=sp.Resolve<IEnumerable<ITestCaseProvider>>();
				foreach (var provider in providers)
					foreach (var tc in provider.GetTestCases())
						await provider.Execute(tc);
			});
			return sc;
		}
	}
}
