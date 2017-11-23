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
using System.Linq;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Linq;

namespace SF.Sys.Tests.ValueAsserts
{

	class DefaultValueValidatorProvider : IValueValidatorProvider
	{
		ITestAssert TestAssert { get; }
		public DefaultValueValidatorProvider(ITestAssert TestAssert)
		{
			this.TestAssert = TestAssert;
		}

		class DefaultValueValidator<T> : IValueValidator<T>
		{
			ITestAssert TestAssert { get; }
			ValidationAttribute[] Validations { get; }

			public DefaultValueValidator(ITestAssert TestAssert,PropertyInfo Prop)
			{
				this.TestAssert = TestAssert;
				this.Validations = 
					Prop?.GetCustomAttributes()
					.Where(a => a is ValidationAttribute)
					.Cast<ValidationAttribute>()
					.ToArray()
					?? Array.Empty<ValidationAttribute>();

			}

			public TestResult Validate(string Source,T Value)
			{
				var ctx = new ValidationContext(this);
				foreach (var v in Validations)
				{
					var re = v.GetValidationResult(Value, ctx);
					if (re == ValidationResult.Success)
						continue;
					return new ValidateResult<T>(Source,Value, re.ErrorMessage + "," + re.MemberNames.Join(","));
				}
				return TestResult.Success;
			}
		}
		public IValueValidator<T> GetValidator<T>(PropertyInfo Prop)
		{
			return new DefaultValueValidator<T>(TestAssert, Prop);
		}
	}
}
