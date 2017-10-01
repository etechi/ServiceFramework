using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class JsonDataAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
			return new SystemAttributeBuilder(
				typeof(JsonDataAttribute).GetConstructor(Array.Empty<Type>())
				);
		}
	}

}
