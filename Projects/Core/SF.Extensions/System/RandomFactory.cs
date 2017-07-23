using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace System
{
	public static class RandomFactory
	{
		static Random RandomSeed { get; } = new Random();
		public static Random Create(int Seed=0)
		{
			if(Seed==0)
				lock(RandomSeed)
					Seed = RandomSeed.Next();
			return new Random(Seed);
		}
	}

}
