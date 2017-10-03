using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class KeyAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
			return new CustomAttributeExpression(
				typeof(KeyAttribute).GetConstructor(Array.Empty<Type>())
				);
		}
	}

}
