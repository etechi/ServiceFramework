using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators
{
	public class RequiredAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
			return new CustomAttributeExpression(
				typeof(RequiredAttribute).GetConstructor(Array.Empty<Type>()),
				Array.Empty<object>()
				);
		}
	}

}