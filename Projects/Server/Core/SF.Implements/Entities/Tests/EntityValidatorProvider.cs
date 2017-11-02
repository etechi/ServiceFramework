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
			Action<TExpect, TTest, string, List<TestResult>> FuncValidator { get; }
			public EntitytValidator(Action<TExpect, TTest, string, List<TestResult>> FuncValidator)
			{
				this.FuncValidator = FuncValidator;

			}
			protected TestResult Validate(TExpect Expect, TTest Test)
			{
				var ars = new List<TestResult>();
				var ident = typeof(TTest).FullName+":"+Entity<TTest>.GetIdentString(Test);
				FuncValidator(Expect, Test, ident ,ars);
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
						).IsNotNull();
		
		protected virtual bool SkipProperty(PropertyInfo p)
		{
			return false;
		}
		static TestResult Validate<T>(IValueAssert<T> Assert, T expect,T test, string testIdent, PropertyInfo p)
		{
			return Assert.Assert(testIdent+":"+p.Name, expect, test);
		}
		static MethodInfo MethodValidate { get; } = typeof(EntityValidatorProvider).GetMethodExt(
						nameof(EntityValidatorProvider.Validate),
						BindingFlags.NonPublic | BindingFlags.Static,
						typeof(IValueAssert<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
						typeof(TypeExtension.GenericTypeArgument),
						typeof(TypeExtension.GenericTypeArgument),
						typeof(string),
						typeof(PropertyInfo)
						).IsNotNull();
		public Action<TExpect, TTest, string,List<TestResult>> GetValidator<TExpect,TTest>()
		{
			var key = (typeof(TExpect), typeof(TTest));
			if (FuncCache.TryGetValue(key, out var ne))
				return (Action<TExpect, TTest, string, List<TestResult>>)ne;

			var argExpect = Expression.Parameter(typeof(TExpect));
			var argTest = Expression.Parameter(typeof(TTest));
			var argIdent = Expression.Parameter(typeof(string));
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
								Expression.Call(
									MethodValidate.MakeGenericMethod(p.PropertyType),
									Expression.Constant(vth, valueAssertType),
									argExpect.GetMember(p).To(p.PropertyType),
									argTest.GetMember(p),
									argIdent,
									Expression.Constant(p)
									)
								)
					).Compile<Action<TExpect, TTest, string,List<TestResult>>>(argExpect, argTest, argIdent,argResults);
			return (Action<TExpect, TTest, string, List<TestResult>>)FuncCache.GetOrAdd(key, func);
		}

	}
}
