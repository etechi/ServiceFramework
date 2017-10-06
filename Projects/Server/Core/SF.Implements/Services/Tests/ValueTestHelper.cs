using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SF.Services.Tests
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

		public TestResult Assert(T ExpectValue, T TestValue)
		{
			return TestResult.Merge(
				ValueAsserts.Select(va => va.Assert(ExpectValue, TestValue))
				);
		}

		int SampleSourceSource = 0;
		public T NextSample(T OrgValue,ISampleSeed Seed)
		{
			if (NextSampleGenerators.Length == 0)
				return default(T);
			return NextSampleGenerators[SampleSourceSource++ % SampleGenerators.Length].NextSample(OrgValue,Seed);
		}
		
		public TestResult Validate(T Value)
		{
			return TestResult.Merge(
				Validators.Select(v => v.Validate(Value))
				);			
		}
	}
}
