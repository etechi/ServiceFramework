using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.AttributeValidators
{
	public class RequiredValidator : IValueValidator<object, MaxLengthAttribute>
	{
		public Type ValueType => typeof(object);

		public Type AttrType => typeof(MaxLengthAttribute);

		public string Validate(object value, MaxLengthAttribute Attr)
		{
			if (value == null )
				return Attr.ErrorMessage ?? $"必须提供";
			return null;
		}
	}

}
