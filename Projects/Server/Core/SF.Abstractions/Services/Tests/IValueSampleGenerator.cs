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
	public interface IValueSampleGenerator<T>
	{
		bool NextSampleSupported { get; }
		IEnumerable<T> ValidSamples { get; }
		IEnumerable<T> InvalidSamples { get; }
		T NextSample(T OrgValue,ISampleSeed Seed);
	}
	public interface IValueSampleGeneratorProvider
	{
		IValueSampleGenerator<T> GetGenerator<T>(PropertyInfo Prop);
	}
}
