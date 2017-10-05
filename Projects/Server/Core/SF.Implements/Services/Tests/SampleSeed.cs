using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SF.Services.Tests
{

	public class SampleSeed : ISampleSeed
	{
		volatile int _value;
		public int NextValue()
		{
			return System.Threading.Interlocked.Increment(ref _value);
		}
	}
}
