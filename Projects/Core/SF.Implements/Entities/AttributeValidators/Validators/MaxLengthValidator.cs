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
	public class MaxLengthValidator : IValueValidator<string, MaxLengthAttribute>
	{
		public Type ValueType => typeof(string);

		public Type AttrType => typeof(MaxLengthAttribute);

		public string Validate(string value, MaxLengthAttribute Attr)
		{
			if (value != null && value.Length > Attr.Length)
				return Attr.ErrorMessage ?? $"不能超过{Attr.Length}个字符";
			return null;
		}
	}

}
