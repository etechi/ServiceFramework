using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators
{
	public class MaxLengthAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
			var Length = Attr.Values.Get("Length");
			if (Length == null)
				return null;
			else
				return new CustomAttributeExpression(
					typeof(MaxLengthAttribute).GetConstructor(new[] { typeof(int) }),
					new object[] { Convert.ToInt32(Length) }
				);
		}
	}

}
