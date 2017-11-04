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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SF.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class DefaultQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;

		class DefaultConverter : IEntityPropertyQueryConverter
		{
			public static DefaultConverter Instance { get; } = new DefaultConverter();
			public Type TempFieldType => null;

			public Expression SourceToDestOrTemp(Expression src, int Level, PropertyInfo srcProp,PropertyInfo dstProp)
			{
				var exp=src.GetMember(srcProp);
				return exp.Type == dstProp.PropertyType ? exp : exp.To(dstProp.PropertyType);
			}
		}

		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType,PropertyInfo DataModelProperty, Type EntityType, PropertyInfo EntityProperty, QueryMode QueryMode)
		{
			if (DataModelProperty == null)
				return null;
			if ((DataModelProperty.GetCustomAttribute<ForeignKeyAttribute>() != null && DataModelProperty.GetCustomAttribute<KeyAttribute>() == null) ||
				DataModelProperty.GetCustomAttribute<InversePropertyAttribute>() != null)
				return null;
			if (!DataModelProperty.PropertyType.CanSimpleConvertTo(EntityProperty.PropertyType))
				return null;

			//src = Expression.Convert(src, dstProp.PropertyType);


			//var src = (Expression)Expression.Property(ArgSource, srcProp);
			//		if (dstProp.PropertyType != srcProp.PropertyType)
			//	{
			//		if (srcProp.PropertyType.CanSimpleConvertTo(dstProp.PropertyType))
			//			src = Expression.Convert(src, dstProp.PropertyType);
			//		else
			//			throw new NotSupportedException($"来源字段{SrcType.FullName}.{srcProp.Name} 的类型{srcProp.PropertyType} 和目标字段 {DstType.FullName}.{dstProp.Name} 的类型{dstProp.PropertyType}不兼容");
			//	}
			//	DstBindings.Add(Expression.Bind(dstProp, src));
			//}
			return DefaultConverter.Instance;
		}
	}

}
