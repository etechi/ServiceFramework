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
	public class RangeValidator : IValueValidator<int, RangeAttribute>
	{
		public Type ValueType => typeof(int);

		public Type AttrType => typeof(RangeAttribute);

		public string Validate(int value, RangeAttribute Attr)
		{
			if (value < Convert.ToInt32(Attr.Minimum) || value> Convert.ToInt32(Attr.Maximum))
				return Attr.ErrorMessage ?? $"必须在{Attr.Minimum} - {Attr.Maximum}之间";
			return null;
		}
	}

}
