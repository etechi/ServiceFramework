﻿using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Reflection;
using SF.Services.Tests.NumberValueTypes;
using SF.Core.Times;
using System.ComponentModel.DataAnnotations;

namespace SF.Services.Tests.ValueAsserts
{

	class DefaultValueAssertProvider : IValueAssertProvider
	{
		ITestAssert TestAssert { get; }
		class DefaultValueAssert<T> : IValueAssert<T>
		{
			ITestAssert TestAssert { get; }
			public DefaultValueAssert(ITestAssert TestAssert)
			{
				this.TestAssert = TestAssert;
			}
			public AssertResult Assert(T ExpectValue, T TestValue)
			{
				if (Comparer<T>.Default.Compare(ExpectValue, TestValue) == 0)
					return AssertResult.Success;
				else
					return new AssertResult<T>(ExpectValue, TestValue);

			}
		}
		public IValueAssert<T> GetValueAssert<T>(PropertyInfo Prop)
		{
			return new DefaultValueAssert<T>(TestAssert);
		}
	}
}
