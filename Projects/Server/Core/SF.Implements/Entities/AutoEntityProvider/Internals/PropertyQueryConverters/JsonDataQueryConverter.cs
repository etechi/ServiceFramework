#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
