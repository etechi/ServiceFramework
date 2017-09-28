using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class TableAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
				return new SystemAttributeBuilder(
					typeof(TableAttribute).GetConstructor(new[] { typeof(string) }),
					new[] {Attr.Values?.Get("Name")}
					
					);
		}
	}

}
