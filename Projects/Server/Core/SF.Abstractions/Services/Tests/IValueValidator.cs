using SF.Auth;
using SF.Metadata;
using SF.Core.NetworkService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace SF.Services.Tests
{
	public class ValidateResult
	{
		public static ValidateResult Success { get; } = new ValidateResult(null,null);
		public string Message { get; }
		public ValidateResult(string message)
		{
			this.Message = message;
		}
	}

	public class ValidateResult<T>: ValidateResult
	{
		public T Value { get; }
		public ValidateResult(T Value, string message):base(message)
		{
			this.Value = Value;

		}
	}
	public interface IValueValidator<T>
	{
		ValidateResult Validate(T Value);
	}
	public interface IValueValidatorProvider
	{
		IValueValidator<T> GetValidator<T>(PropertyInfo Prop);
	}

}
