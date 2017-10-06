
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
namespace SF.Entities.Tests
{
	class EntityValidatorProvider
	{
		public class EntitytValidator<TExpect,TTest> 
		{
			Action<TExpect, TTest, List<TestResult>> FuncValidator { get; }
			public EntitytValidator(Action<TExpect, TTest, List<TestResult>> FuncValidator)
			{
				this.FuncValidator = FuncValidator;

			}
			protected TestResult Validate(TExpect Expect, TTest Test)
			{
				var ars = new List<TestResult>();
				FuncValidator(Expect, Test, ars);
				return TestResult.Merge(ars);
			}
		}

		IValueTestHelperCache ValueTestHelperCache { get; }

		System.Collections.Concurrent.ConcurrentDictionary<(Type,Type), object> FuncCache { get; } =
			new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), object>();

		public EntityValidatorProvider(IValueTestHelperCache ValueTestHelperCache)
		{
			this.ValueTestHelperCache = ValueTestHelperCache;
		}

		static MethodInfo MethodGetHelper { get; } = typeof(IValueTestHelperCache).GetMethodExt(
						nameof(IValueTestHelperCache.GetHelper),
						BindingFlags.Public | BindingFlags.Instance,
						typeof(PropertyInfo)
						);
		
		protected virtual bool SkipProperty(PropertyInfo p)
		{
			return false;
		}
		public Action<TExpect, TTest, List<TestResult>> GetValidator<TExpect,TTest>()
		{
			var key = (typeof(TExpect), typeof(TTest));
			if (FuncCache.TryGetValue(key, out var ne))
				return (Action<TExpect, TTest, List<TestResult>>)ne;

			var argExpect = Expression.Parameter(typeof(TExpect));
			var argTest = Expression.Parameter(typeof(TTest));
			var argResults = Expression.Parameter(typeof(List<TestResult>));
			var func = Expression.Block(
						from p in typeof(TTest).AllPublicInstanceProperties()
						where Entity<TTest>.KeyProperties.All(kp => kp.Name != p.Name) && !SkipProperty(p)
						let vthType = typeof(IValueTestHelper<>).MakeGenericType(p.PropertyType)
						let valueAssertType = typeof(IValueAssert<>).MakeGenericType(p.PropertyType)
						let vth = MethodGetHelper.MakeGenericMethod(p.PropertyType).Invoke(ValueTestHelperCache, new[] { p })

						select
							argResults.CallMethod(
								typeof(List<>).MakeGenericType<TestResult>().GetMethod(nameof(List<int>.Add)),
								Expression.Constant(vth, valueAssertType).CallMethod(
									valueAssertType.GetMethod(nameof(IValueAssert<int>.Assert)),
									argExpect.GetMember(p).To(p.PropertyType),
									argTest.GetMember(p)
									)
								)
					).Compile<Action<TExpect, TTest, List<TestResult>>>(argExpect, argTest, argResults);
			return (Action<TExpect, TTest, List<TestResult>>)FuncCache.GetOrAdd(key, func);
		}

	}
}
