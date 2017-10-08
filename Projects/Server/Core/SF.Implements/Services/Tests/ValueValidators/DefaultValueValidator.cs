using System;

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
