using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.AttributeGenerators
{
	public class TableAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeExpression Generate(IAttribute Attr)
		{
				return new CustomAttributeExpression(
					typeof(TableAttribute).GetConstructor(new[] { typeof(string) }),
					new[] {Attr.Values?.Get("Name")}
					
					);
		}
	}

}
