using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class ColumnAttributeGenerator : IDataModelAttributeGenerator
	{
		public CustomAttributeBuilder Generate(IAttribute Attr)
		{
			var name = Attr.Values.Get("Name");
			if(name==null)
				return new CustomAttributeBuilder(
					typeof(ColumnAttribute).GetConstructor(Array.Empty<Type>()),
					Array.Empty<object>(),
					new[] { typeof(ColumnAttribute).GetProperty("Order") },
					new[] { (object)Convert.ToInt32(Attr.Values.Get("Order")) }
					);
			else
				return new CustomAttributeBuilder(
					typeof(ColumnAttribute).GetConstructor(new[] { typeof(string) }),
					new[] { (object)name },
					new[] { typeof(ColumnAttribute).GetProperty("Order") },
					new[] { (object)Convert.ToInt32(Attr.Values.Get("Order")) }
					);
		}
	}

}
