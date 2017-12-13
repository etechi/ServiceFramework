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
using System.Linq.Expressions;
using System.Reflection;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys.Tests;

namespace SF.Sys.Entities.Tests
{
	//public interface IEntityPropertyValidator<TExpectEntityType, TTestEntityType>
	//{

	//}
	//public interface IEntityPropertyValidator<TExpectEntityType, TExpectValueType,TTestEntityType,TTestValueType> : 
	//	IEntityPropertyValidator<TExpectEntityType, TTestEntityType>
	//{
	//	TestResult Validate(
	//		IValueAssert<TExpectValueType> ExpectAssert, 
	//		IValueAssert<TTestValueType> TestAssert,
	//		string TestIdent,
	//		TExpectValueType ExpectValue, 
	//		TTestValueType TestValue
	//		);
	//}
	//public interface IEntityPropertyValidatorProvider
	//{
	//	IEntityPropertyValidator<TExpectEntityType, TTestEntityType> GetValidator<TExpectEntityType, TTestEntityType>(
	//		PropertyInfo ExpectPropInfo, 
	//		PropertyInfo TestPropInfo
	//		);
	//}
	//public class DefaultEntityPropertyValidatorProvider : IEntityPropertyValidatorProvider
	//{
	//	class IEntityPropertyValidator<TExpectEntityType, TTestEntityType, TValueType> :
	//		IEntityPropertyValidator<TExpectEntityType, TValueType, TTestEntityType, TValueType>
	//	{
	//		PropertyInfo ExpectPropInfo { get; }
	//		PropertyInfo TestPropInfo { get; }

	//		public TestResult Validate(
	//			IValueAssert<TValueType> ExpectAssert,
	//			IValueAssert<TValueType> TestAssert,
	//			string TestIdent,
	//			TValueType ExpectValue, 
	//			TValueType TestValue
	//			)
	//		{
	//			return TestAssert.Assert(TestIdent + ":" + TestPropInfo.Name, ExpectValue, TestValue);
	//		}
	//	}
	//	public IEntityPropertyValidator<TExpectEntityType, TTestEntityType> GetValidator<TExpectEntityType, TTestEntityType>(PropertyInfo ExpectPropInfo, PropertyInfo TestPropInfo)
	//	{
	//		if (ExpectPropInfo.PropertyType != TestPropInfo.PropertyType)
	//			return null;

	//	}
	//}
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
				var ident = typeof(TTest).FullName+":"+Entity<TTest>.GetIdentValues(Test);
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
						let expectProp = typeof(TExpect).GetProperty(p.Name) ?? 
							throw new InvalidOperationException($"{typeof(TExpect)}中找不到被测属性{p.Name}")
						let expectValue = argExpect.GetMember(expectProp)
						let convertedExpectValue= p.PropertyType==expectProp.PropertyType?
								expectValue : 
								throw new InvalidOperationException($"被测属性{typeof(TTest)}.{p.Name}类型为{p.PropertyType}和目标属性{typeof(TExpect)}.{p.Name}的类型{expectProp.PropertyType}不符")
						select
							argResults.CallMethod(
								typeof(List<>).MakeGenericType<TestResult>().GetMethod(nameof(List<int>.Add)),
								Expression.Call(
									MethodValidate.MakeGenericMethod(p.PropertyType),
									Expression.Constant(vth, valueAssertType),
									convertedExpectValue,
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
