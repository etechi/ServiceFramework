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
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SF.Sys.Linq.Expressions;
using SF.Sys.Reflection;
using SF.Sys.Annotations;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class EntityNameQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;
			
		class MultipleRelationConverter : IEntityPropertyQueryConverter
		{
			public Type TempFieldType => null;

			static Expression NullString { get; } = Expression.Constant(null, typeof(string));
			public PropertyInfo ForeignKeyProp { get; }
			public PropertyInfo NullableHasValue { get; }
			public Expression NullForeignKeyValue { get; }

			public PropertyInfo SingleRelationProp { get; }
			public PropertyInfo NameProp { get; }

			public bool CanBeNull { get; }
			public MultipleRelationConverter(PropertyInfo ForeignKeyProp ,PropertyInfo SingleRelationProp,PropertyInfo NameProp,bool CanBeNull)
			{
				this.SingleRelationProp = SingleRelationProp;
				this.NameProp = NameProp;

				this.ForeignKeyProp = ForeignKeyProp;
				if (this.CanBeNull = CanBeNull)
				{
					if (ForeignKeyProp.PropertyType.IsClass)
						NullForeignKeyValue = Expression.Constant(null, ForeignKeyProp.PropertyType);
					else
						NullableHasValue = ForeignKeyProp.PropertyType.GetProperty(nameof(Nullable<int>.HasValue));
				}
			}


			public Expression SourceToDestOrTemp(
				Expression src, 
				int level, 
				IPropertySelector PropertySelector, 
				PropertyInfo srcProp, 
				PropertyInfo dstProp
				)
			{
				if (!CanBeNull)
					return src.GetMember(SingleRelationProp).GetMember(NameProp);
				else if (ForeignKeyProp.PropertyType.IsClass)
					return Expression.Condition(
						src.GetMember(ForeignKeyProp).Equal(NullForeignKeyValue),
						NullString,
						src.GetMember(SingleRelationProp).GetMember(NameProp)
						);
				else
					return Expression.Condition(
						src.GetMember(ForeignKeyProp).GetMember(NullableHasValue),
						src.GetMember(SingleRelationProp).GetMember(NameProp),
						NullString
						);
			}
		}
		
		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType, PropertyInfo DataModelProperty, Type EntityType, PropertyInfo EntityProperty,QueryMode QueryMode)
		{
			if (QueryMode == QueryMode.Edit)
				return null;
			if (DataModelProperty != null)
				return null;
			if (EntityProperty.PropertyType != typeof(string))
				return null;

			//确认当前字段是否是实体名称
			var fkField=EntityProperty.ReflectedType.AllPublicInstanceProperties().FirstOrDefault(p =>
			{
				var a = p.GetCustomAttribute<EntityIdentAttribute>();
				return a != null && a.NameField == EntityProperty.Name;
			});
			if (fkField == null)
				return null;

			//查找外键
			var modelFkField = DataModelType.GetProperty(fkField.Name);
			if (modelFkField == null)
				return null;

			//查找关系
			var modelSingleRelationProp = DataModelType.AllPublicInstanceProperties().FirstOrDefault(p =>
			  {
				  var fk=p.GetCustomAttribute<ForeignKeyAttribute>();
				  return fk != null && fk.Name == fkField.Name;
			  });
			if (modelSingleRelationProp == null)
				return null;

			var nameProp = modelSingleRelationProp.PropertyType.GetProperty("Name");
			if (nameProp == null)
				return null;

			//是否有可能为空
			var canBeNull =
				modelFkField.PropertyType.IsClass && !modelFkField.IsDefined(typeof(RequiredAttribute)) ||
				modelFkField.PropertyType.IsGeneric() && modelFkField.PropertyType.GetGenericTypeDefinition()==typeof(Nullable<>);

			//可空对象EF不支持载入关联对象
			if (canBeNull)
				return null;
			return new MultipleRelationConverter(
				modelFkField,
				modelSingleRelationProp, 
				nameProp,
				canBeNull				
				);
		}
	}

}
