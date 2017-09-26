using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class KeyAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeBuilder Generate(IAttribute Attr)
		{
			return new CustomAttributeBuilder(
				typeof(KeyAttribute).GetConstructor(Array.Empty<Type>()),
				Array.Empty<object>()
				);
		}
	}

}
