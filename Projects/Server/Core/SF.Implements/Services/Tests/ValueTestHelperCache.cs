using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;

namespace SF.Services.Tests
{
	class ValueTestHelperCache : IValueTestHelperCache
	{
		System.Collections.Concurrent.ConcurrentDictionary<(Type, PropertyInfo), object> Helpers { get; } 
			= new System.Collections.Concurrent.ConcurrentDictionary<(Type, PropertyInfo), object>();

		IValueAssertProvider[] ValueAssertProviders { get; }
		IValueSampleGeneratorProvider[] ValueSampleGeneratorProviders { get; }
		IValueValidatorProvider[] ValueValidatorProviders { get; }
		public ValueTestHelperCache(
			IEnumerable<IValueAssertProvider> ValueAssertProviders,
			IEnumerable<IValueSampleGeneratorProvider> ValueSampleGeneratorProviders,
			IEnumerable<IValueValidatorProvider> ValueValidatorProviders
			)
		{
			this.ValueAssertProviders = ValueAssertProviders.ToArray();
			this.ValueSampleGeneratorProviders = ValueSampleGeneratorProviders.ToArray();
			this.ValueValidatorProviders = ValueValidatorProviders.ToArray();
		}
		public IValueTestHelper<T> GetHelper<T>(PropertyInfo Prop)
		{
			var key = (typeof(T), Prop);
			if (Helpers.TryGetValue(key, out var h))
				return (IValueTestHelper<T>)h;
			return (IValueTestHelper<T>)Helpers.GetOrAdd(key, new ValueTestHelper<T>(
				ValueAssertProviders.Select(p => p.GetValueAssert<T>(Prop)).Where(p => p != null).ToArray(),
				ValueSampleGeneratorProviders.Select(p => p.GetGenerator<T>(Prop)).Where(p => p != null).ToArray(),
				ValueValidatorProviders.Select(p => p.GetValidator<T>(Prop)).Where(p => p != null).ToArray()
				));
		}
	}
}
