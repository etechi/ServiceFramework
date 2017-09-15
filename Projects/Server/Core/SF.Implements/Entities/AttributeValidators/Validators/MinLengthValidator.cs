using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities.AttributeValidators
{


	public class MinLengthValidator : IValueValidator<string, MinLengthAttribute>
	{
		public Type ValueType => typeof(string);

		public Type AttrType => typeof(MinLengthAttribute);

		public string Validate(string value, MinLengthAttribute Attr)
		{
			if (value != null && value.Length < Attr.Length)
				return Attr.ErrorMessage ?? $"不少于{Attr.Length}个字符";
			return null;
		}
	}
}
