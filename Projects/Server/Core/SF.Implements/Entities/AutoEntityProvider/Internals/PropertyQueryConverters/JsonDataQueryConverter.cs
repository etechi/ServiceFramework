using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using SF.Core.Serialization;
using SF.Metadata;

namespace SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class JsonDataQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		IJsonSerializer JsonSerializer { get; }
		public JsonDataQueryConverterProvider(IJsonSerializer JsonSerializer)
		{
			this.JsonSerializer = JsonSerializer;
		}

		public int Property => -1;

		class JsonDataQueryConverter<T> : IEntityPropertyQueryConverter<string, T>
		{
			IJsonSerializer JsonSerializer { get; }
			public JsonDataQueryConverter(IJsonSerializer JsonSerializer)
			{
				this.JsonSerializer = JsonSerializer;
			}
			public Type TempFieldType => typeof(string);

			public Expression SourceToTemp(Expression src, PropertyInfo srcProp)
			{
				return src.GetMember(srcProp);
			}

			public Task<T> TempToDest(object src, string value)
			{
				return Task.FromResult(value.IsNullOrEmpty() ? default(T) : JsonSerializer.Deserialize<T>(value));
			}
		}

		public IEntityPropertyQueryConverter GetPropertyConverter(PropertyInfo DataModelProperty, PropertyInfo EntityProperty)
		{
			if (DataModelProperty==null ||
				EntityProperty==null||
				DataModelProperty.PropertyType != typeof(string) || 
				!DataModelProperty.IsDefined(typeof(JsonDataAttribute))
				)
				return null;
			return (IEntityPropertyQueryConverter)Activator.CreateInstance(
				typeof(JsonDataQueryConverter<>).MakeGenericType(EntityProperty.PropertyType),
				JsonSerializer
				);
		}
	}

}
