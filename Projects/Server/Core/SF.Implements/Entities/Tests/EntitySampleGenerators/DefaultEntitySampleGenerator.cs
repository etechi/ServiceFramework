
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
using SF.Metadata;

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
				GetSampleInit<TEditable>()
				);
		}

		static MethodInfo MethodGetHelper { get; } = typeof(IValueTestHelperCache).GetMethodExt(
				nameof(IValueTestHelperCache.GetHelper),
				BindingFlags.Public | BindingFlags.Instance,
				typeof(PropertyInfo)
				);

		System.Collections.Concurrent.ConcurrentDictionary<Type, object> NextSampleCache { get; } = 
			new System.Collections.Concurrent.ConcurrentDictionary<Type, object>();

		Action<TEditable, ISampleSeed> GetSampleInit<TEditable>()
		{
			if (NextSampleCache.TryGetValue(typeof(TEditable), out var ne))
				return (Action<TEditable, ISampleSeed>)ne;
			var argEditable = Expression.Parameter(typeof(TEditable));
			var argSeed = Expression.Parameter(typeof(ISampleSeed));
			var func= Expression.Block(
						from p in typeof(TEditable).AllPublicInstanceProperties()
						where Entity<TEditable>.KeyProperties.All(kp => kp.Name != p.Name) && !SkipProperty(p)
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
										p.PropertyType,
										typeof(ISampleSeed)
										),
									argEditable.GetMember(p),
									argSeed
									)
								)
					).Compile<Action<TEditable, ISampleSeed>>(argEditable, argSeed);
			return (Action<TEditable, ISampleSeed>)NextSampleCache.GetOrAdd(typeof(TEditable), func);
		}
		bool SkipProperty(PropertyInfo p)
		{
			if (p.PropertyType != typeof(string))
				return false;

			if ((from ip in p.ReflectedType.AllPublicInstanceProperties()
				 let a = ip.GetCustomAttribute<EntityIdentAttribute>()
				 where a != null && a.NameField == p.Name
				 select a
				).Any())
				return true;

			return false;
		}

		class DefaultEntitySampleGenerator<TEditable> : IEntitySampleGenerator<TEditable>
		{
			Action<TEditable, ISampleSeed> FuncSampleInit { get; }
			public DefaultEntitySampleGenerator(Action<TEditable, ISampleSeed> FuncSampleInit)
			{
				this.FuncSampleInit = FuncSampleInit;
			}
			public int Priority => 0;

			public bool NextSampleSupported => true;

			public IEnumerable<TEditable> ValidSamples => Enumerable.Empty<TEditable>();

			public IEnumerable<TEditable> InvalidSamples => Enumerable.Empty<TEditable>();

			public TEditable NextSample(TEditable OrgValue, ISampleSeed Seed)
			{
				var NewValue = Entity<TEditable>.GetKey<TEditable>(OrgValue);
				FuncSampleInit(NewValue, Seed);
				return NewValue;
			}
		}
		
	}
}
