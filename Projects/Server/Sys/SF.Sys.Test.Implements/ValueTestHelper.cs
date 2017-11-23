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
using System.Linq;

namespace SF.Sys.Tests
{
	class ValueTestHelper<T> : IValueTestHelper<T>
	{
		IValueAssert<T>[] ValueAsserts { get; }
		IValueSampleGenerator<T>[] SampleGenerators { get; }
		IValueSampleGenerator<T>[] NextSampleGenerators { get; }
		IValueValidator<T>[] Validators { get; }

		public IEnumerable<T> ValidSamples => 
			SampleGenerators.SelectMany(g => g.ValidSamples);

		public IEnumerable<T> InvalidSamples =>
			SampleGenerators.SelectMany(g => g.InvalidSamples);

		public bool NextSampleSupported => NextSampleGenerators.Length > 0;

		public ValueTestHelper(
			IValueAssert<T>[] ValueAsserts,
			IValueSampleGenerator<T>[] SampleGenerators,
			IValueValidator<T>[] Validators
			){
			this.ValueAsserts = ValueAsserts;
			this.SampleGenerators = SampleGenerators;
			this.Validators = Validators;
			this.NextSampleGenerators = SampleGenerators.Where(s => s.NextSampleSupported).ToArray();
		}

		public TestResult Assert(string Source,T ExpectValue, T TestValue)
		{
			return TestResult.Merge(
				ValueAsserts.Select(va => va.Assert(Source,ExpectValue, TestValue))
				);
		}

		int SampleSourceSource = 0;
		public T NextSample(T OrgValue,ISampleSeed Seed)
		{
			if (NextSampleGenerators.Length == 0)
				return default(T);
			return NextSampleGenerators[SampleSourceSource++ % SampleGenerators.Length].NextSample(OrgValue,Seed);
		}
		
		public TestResult Validate(string Source,T Value)
		{
			return TestResult.Merge(
				Validators.Select(v => v.Validate(Source,Value))
				);			
		}
	}
}
