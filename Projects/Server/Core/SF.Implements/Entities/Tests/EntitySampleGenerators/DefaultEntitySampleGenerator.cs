
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.Services.Tests;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.Entities.Tests.EntitySampleGenerators
{
	class DefaultEntitySampleGeneratorProvider : IEntitySampleGeneratorProvider
	{
		IValueTestHelperCache ValueTestHelperCache { get; }
		public DefaultEntitySampleGeneratorProvider(IValueTestHelperCache ValueTestHelperCache)
		{
			this.ValueTestHelperCache = ValueTestHelperCache;
		}

		public IEntitySampleGenerator<TEditable> GetSampleGenerator<TEditable>()
		{
			return new DefaultEntitySampleGenerator<TEditable>(
				GetNextSample<TEditable>()
				);
		}

		static MethodInfo MethodGetHelper { get; } = typeof(IValueTestHelperCache).GetMethodExt(
				nameof(IValueTestHelperCache.GetHelper),
				BindingFlags.Public | BindingFlags.Instance,
				typeof(PropertyInfo)
				);

		System.Collections.Concurrent.ConcurrentDictionary<Type, object> NextSampleCache { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();

		Func<TEditable, ISampleSeed, TEditable> GetNextSample<TEditable>()
		{
			if (NextSampleCache.TryGetValue(typeof(TEditable), out var ne))
				return (Func<TEditable, ISampleSeed, TEditable>)ne;
			var argEditable = Expression.Parameter(typeof(TEditable));
			var argSeed = Expression.Parameter(typeof(ISampleSeed));
			var func= Expression.Block(
						from p in typeof(TEditable).AllPublicInstanceProperties()
						where Entity<TEditable>.KeyProperties.All(kp => kp.Name != p.Name)
						let vthType = typeof(IValueTestHelper<>).MakeGenericType(p.PropertyType)
						let valueSampleGeneratorType = typeof(IValueSampleGenerator<>).MakeGenericType(p.PropertyType)
						let vth = MethodGetHelper.MakeGenericMethod(p.PropertyType).Invoke(ValueTestHelperCache, new[] { p })
						select
							argEditable.CallMethod(
								p.GetSetMethod(),
								Expression.Constant(vth, valueSampleGeneratorType).CallMethod(
									valueSampleGeneratorType.GetMethodExt(
										nameof(IValueSampleGenerator<int>.NextSample),
										BindingFlags.Public | BindingFlags.Instance,
										typeof(TypeExtension.GenericTypeArgument),
										typeof(ISampleSeed)
										),
									argEditable.GetMember(p),
									argSeed
									)
								)
					).Compile<Func<TEditable, ISampleSeed, TEditable>>(argEditable, argSeed);
			return (Func<TEditable, ISampleSeed, TEditable>)NextSampleCache.GetOrAdd(typeof(TEditable), func);
		}
		

		class DefaultEntitySampleGenerator<TEditable> : IEntitySampleGenerator<TEditable>
		{
			Func<TEditable, ISampleSeed, TEditable> FuncNextSample { get; }
			public DefaultEntitySampleGenerator(Func<TEditable, ISampleSeed, TEditable> FuncNextSample)
			{
				this.FuncNextSample = FuncNextSample;
			}
			public int Priority => 0;

			public bool NextSampleSupported => true;

			public IEnumerable<TEditable> ValidSamples => Enumerable.Empty<TEditable>();

			public IEnumerable<TEditable> InvalidSamples => Enumerable.Empty<TEditable>();

			public TEditable NextSample(TEditable OrgValue, ISampleSeed Seed)
			{
				return FuncNextSample(OrgValue, Seed);
			}
		}
		
	}
}
