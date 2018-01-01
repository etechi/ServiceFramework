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
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SF.Sys.Reflection;
using SF.Sys.Annotations;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class MultipleRelationIdentQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;
			
		class MultipleRelationConverter : IEntityPropertyQueryConverter
		{
			static MethodInfo MethodSelect { get; } = typeof(Enumerable)
				.GetMethodExt(
					nameof(Enumerable.Select),
					BindingFlags.Public | BindingFlags.Static,
					typeof(IEnumerable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
					typeof(Func<,>).MakeGenericType<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>()
				);
			static MethodInfo MethodOrderBy { get; } = typeof(Enumerable)
				.GetMethodExt(
					nameof(Enumerable.OrderBy),
					BindingFlags.Public | BindingFlags.Static,
					typeof(IEnumerable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
					typeof(Func<,>).MakeGenericType<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>()
				);
			MethodInfo MethodSelectSpec { get; }
			MethodInfo MethodOrderBySpec { get; }
			PropertyInfo SrcOrderItemProperty { get; }
			ParameterExpression ArgModel { get; }
			PropertyInfo KeyProperty { get; }

			public Type TempFieldType => null;

			public MultipleRelationConverter( PropertyInfo KeyProperty, PropertyInfo SrcOrderItemProperty)
			{
				this.KeyProperty = KeyProperty;
				MethodSelectSpec = MethodSelect.MakeGenericMethod(KeyProperty.ReflectedType, KeyProperty.PropertyType);
				MethodOrderBySpec = SrcOrderItemProperty==null?null:MethodOrderBy.MakeGenericMethod(SrcOrderItemProperty.ReflectedType, SrcOrderItemProperty.PropertyType);
				this.SrcOrderItemProperty = SrcOrderItemProperty;
				this.ArgModel = Expression.Parameter(KeyProperty.ReflectedType);
			}

			public Expression SourceToDestOrTemp(Expression src,int Level, IPropertySelector PropertySelector, PropertyInfo srcProp,PropertyInfo dstProp)
			{
				if (Level == 3)
					return null;
				var exp = src.GetMember(srcProp);
				if (SrcOrderItemProperty != null)
					exp = Expression.Call(
						null,
						MethodOrderBySpec,
						exp,
						Expression.Lambda(
							ArgModel.GetMember(SrcOrderItemProperty),
							ArgModel
						)
					);
				exp = Expression.Call(
					null,
					MethodSelectSpec,
					exp,
					Expression.Lambda(
						ArgModel.GetMember(KeyProperty),
						ArgModel
					)
				);

				return exp;
			}
		}

		static string Desc(PropertyInfo EntityProperty, PropertyInfo DataModelProperty)
			=> $"一对多关系ID查询支持{EntityProperty.DeclaringType.FullName}.{EntityProperty.Name}=>{DataModelProperty.DeclaringType.FullName}.{DataModelProperty.Name}";

		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType, PropertyInfo DataModelProperty, Type EntityType, PropertyInfo EntityProperty, QueryMode QueryMode)
		{
			if (QueryMode == QueryMode.Summary)
				return null;
			if (DataModelProperty == null)
				return null;
			if (!DataModelProperty.PropertyType.IsGenericTypeOf(typeof(ICollection<>)))
				return null;

			if (!EntityProperty.PropertyType.IsGenericTypeOf(typeof(IEnumerable<>))) 
				return null;


			var childModelType = DataModelProperty.PropertyType.GenericTypeArguments[0];
			var EntityItemIdentType = EntityProperty.PropertyType.GenericTypeArguments[0];
			if (!EntityItemIdentType.IsConstType() || EntityProperty.GetCustomAttribute<EntityIdentAttribute>() == null)
				return null;

			var invesetPropertyAttr = DataModelProperty.GetCustomAttribute<InversePropertyAttribute>();
			if (invesetPropertyAttr == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的实体属性{DataModelProperty.Name}没有定义InverseProperty特性");

			var ForeignProperty = childModelType.GetProperty(invesetPropertyAttr.Property);
			if (ForeignProperty == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象未定义外键属性{invesetPropertyAttr.Property}");

			if (ForeignProperty.PropertyType != DataModelProperty.ReflectedType)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象外键属性类型({ForeignProperty.PropertyType.FullName})和父对象类型不同");

			var ForeignKeyPropAttr = ForeignProperty.GetCustomAttribute<ForeignKeyAttribute>();
			if (ForeignKeyPropAttr == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象外键属性未定义外键字段特性(ForeignKeyAttribute)");

			var ForeignKeyProp = childModelType.GetProperty(ForeignKeyPropAttr.Name);
			if (ForeignKeyProp == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象中不到ForeignKeyAttribute指定的外键字段{ForeignKeyPropAttr.Name}");

			var childModelKeys = ((IReadOnlyList<PropertyInfo>)typeof(Entity<>)
				.MakeGenericType(childModelType)
				.GetProperty(nameof(Entity<string>.KeyProperties))
				.GetValue(null)).Where(p => p.Name != ForeignKeyProp.Name).ToArray();
			if(childModelKeys.Length!=1 || childModelKeys[0].PropertyType!= EntityItemIdentType)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 实体和数据对象的主键数目或类型不同");

			var SrcOrderItemProperty = childModelType
				.AllPublicInstanceProperties()
				.FirstOrDefault(p => p.GetCustomAttribute<ItemOrderAttribute>() != null);

			return new MultipleRelationConverter(
				childModelKeys[0],
				SrcOrderItemProperty
				);

		
		}

	}

}
