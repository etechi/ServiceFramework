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
using System.Threading.Tasks;
using SF.Core.Serialization;

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class JsonDataPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public static int DefaultPriority { get; } = -1;
		IJsonSerializer JsonSerializer { get; }
		public JsonDataPropertyModifierProvider(IJsonSerializer jsonSerializer)
		{
			this.JsonSerializer = jsonSerializer;
		}

		class PropertyModifier<T> : IEntityPropertyModifier<T,string>
		{
			IJsonSerializer JsonSerializer { get; }
			public PropertyModifier(IJsonSerializer jsonSerializer)
			{
				this.JsonSerializer = jsonSerializer;
			}
			public int Priority => DefaultPriority;

			public string Execute(IDataSetEntityManager Manager, IEntityModifyContext Context,T value)
			{
				return JsonSerializer.Serialize<T>(value);
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{
			if (DataModelProperty.PropertyType != typeof(string))
				return null;

			var jd = EntityProperty.GetCustomAttribute<JsonDataAttribute>();
			if (jd == null)
				return null;
			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(PropertyModifier<>).MakeGenericType(EntityProperty.PropertyType),
				JsonSerializer
				);
		}
	}
}
