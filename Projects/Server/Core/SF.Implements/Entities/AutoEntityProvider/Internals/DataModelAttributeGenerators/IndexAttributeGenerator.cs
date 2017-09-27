using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations;
using SF.Data;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelAttributeGenerators
{
	public class IndexAttributeGenerator : IDataModelAttributeGenerator
	{
		public SystemAttributeBuilder Generate(IAttribute Attr)
		{
			var props = new List<PropertyInfo>();
			var args = new List<object>();
			var IsClustered = Attr.Values?.Get("IsClustered");
			var IsUnique = Attr.Values?.Get("IsUnique");
			var Name = Attr.Values?.Get("Name");
			var Order = Attr.Values?.Get("Order");
			return new SystemAttributeBuilder(
				typeof(IndexAttribute).GetConstructor(Array.Empty<Type>()),
				Array.Empty<object>(),
				new[]
				{
					typeof(IndexAttribute).GetProperty("IsClustered"),
					typeof(IndexAttribute).GetProperty("IsUnique"),
					typeof(IndexAttribute).GetProperty("Name"),
					typeof(IndexAttribute).GetProperty("Order"),
				},
				new object[]
				{
					Convert.ToBoolean(Attr.Values?.Get("IsClustered")??null),
					Convert.ToBoolean(Attr.Values?.Get("IsUnique")??null),
					Convert.ToString(Attr.Values?.Get("Name")??null),
					Convert.ToInt32(Attr.Values?.Get("Order")??"0")
				}
			);
		}
	}

}
