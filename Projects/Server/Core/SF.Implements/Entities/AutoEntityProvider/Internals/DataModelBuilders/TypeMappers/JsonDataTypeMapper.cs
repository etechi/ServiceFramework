using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.DataModelBuilders.TypeMappers
{
	public class JsonDataTypeMapper : IDataModelTypeMapper
	{
		public JsonDataTypeMapper()
		{
		}

		public int Priority => 0;

		public TypeMapResult MapType(IEntityType EntityType, IProperty Property, Type OrgType)
		{
			var jd = Property?.Attributes?.FirstOrDefault(a => a.Name == typeof(JsonDataAttribute).FullName);
			if (jd == null) return null;
			return new TypeMapResult
			{
				Type = typeof(string),
				Attributes = Property.Attributes
			};
		}
	}
}
