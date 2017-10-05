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

		public void Assert(T ExpectValue, T TestValue)
		{
			ValueAsserts.ForEach(va => va.Assert(ExpectValue, TestValue));
		}

		int SampleSourceSource = 0;
		public T NextSample(T OrgValue,ISampleSeed Seed)
		{
			return NextSampleGenerators[SampleSourceSource++ % SampleGenerators.Length].NextSample(OrgValue,Seed);
		}
		
		public void Validate(T Value)
		{
			Validators.ForEach(v => v.Validate(Value));
		}
	}
}
