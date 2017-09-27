using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class StringLengthAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
			var MaximumLength = Attr.Values.Get("MaximumLength");
			var min = Convert.ToInt32(Attr.Values.Get("MinimumLength"));
			if (MaximumLength == null)
			{
				if (min == 0)
					return null;
				return new SystemAttributeBuilder(
					typeof(StringLengthAttribute).GetConstructor(Array.Empty<Type>()),
					Array.Empty<object>(),
					new PropertyInfo[] { typeof(StringLengthAttribute).GetProperty("MinimumLength", BindingFlags.Public | BindingFlags.Instance) },
					new object[] { min }
					);
			}
			else if (min == 0)
				return new SystemAttributeBuilder(
					typeof(StringLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(MaximumLength) }
				);
			else
				return new SystemAttributeBuilder(
					typeof(StringLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(MaximumLength) },
					new PropertyInfo[] { typeof(StringLengthAttribute).GetProperty("MinimumLength", BindingFlags.Public | BindingFlags.Instance) },
					new object[] { min }
				);
		}
	}

}
