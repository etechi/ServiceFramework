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

using SF.Sys.Annotations;
using SF.Sys.Linq.Expressions;
using SF.Sys.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class MultipleRelationQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;
			
		class MultipleRelationConverter<E,T,R> : IEntityPropertyQueryConverterAsync<IEnumerable<T>,IEnumerable<R>>
		{
			public Type TempFieldType => typeof(IEnumerable<T>);
			IQueryResultBuildHelper<E, T, R> QueryResultBuildHelper { get; }

			//this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector
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
			public MultipleRelationConverter(IQueryResultBuildHelper<E, R> QueryResultBuildHelper, PropertyInfo SrcOrderItemProperty)
			{
				MethodSelectSpec = MethodSelect.MakeGenericMethod(typeof(E),typeof(T));
				MethodOrderBySpec = SrcOrderItemProperty == null ? null : MethodOrderBy.MakeGenericMethod(SrcOrderItemProperty.ReflectedType, SrcOrderItemProperty.PropertyType);

				this.QueryResultBuildHelper = (IQueryResultBuildHelper < E, T, R > )QueryResultBuildHelper;
				this.SrcOrderItemProperty = SrcOrderItemProperty;
			}

			static ParameterExpression ArgItem { get; } = Expression.Parameter(typeof(E));
			public Expression SourceToDestOrTemp(Expression src,int Level, IPropertySelector PropertySelector,PropertyInfo srcProp,PropertyInfo dstProp)
			{
				if (Level <= 0)
					return null;
				var exp = src.GetMember(srcProp);
				if (SrcOrderItemProperty != null)
					exp = Expression.Call(
						null,
						MethodOrderBySpec,
						exp,
						Expression.Lambda(
						body: ArgItem.GetMember(SrcOrderItemProperty),
						parameters: ArgItem
						)
					);
				exp= Expression.Call(
					null,
					MethodSelectSpec,
					exp,
					Expression.Lambda<Func<E, T>>(
						QueryResultBuildHelper.BuildEntityMapper(ArgItem, Level - 1, PropertySelector),
						ArgItem
					)
				);
				
				return exp;

			}

			public async Task<IEnumerable<R>> TempToDest(object src, IEnumerable<T> value, IPropertySelector PropertySelector,int Level)
			{
				if (value == null) return Enumerable.Empty<R>();
				return await QueryResultBuildHelper.GetResultMapper(PropertySelector,Level)(value.ToArray());
			}
		}
		static IEntityPropertyQueryConverter CreateConverter<E,R>(
			IQueryResultBuildHelperCache QueryResultBuildHelperCache, 
			QueryMode QueryMode,
			PropertyInfo SrcOrderItemProperty
			)
		{
			var helper = QueryResultBuildHelperCache.GetHelper<E, R>(QueryMode);
			if (helper == null)
				return null;

			var helperWithTemp = helper.GetType().AllInterfaces().FirstOrDefault(i => i.IsGenericTypeOf(typeof(IQueryResultBuildHelper<,,>)));

			return (IEntityPropertyQueryConverter)Activator.CreateInstance(
				typeof(MultipleRelationConverter<,,>).MakeGenericType(
					typeof(E),
					helperWithTemp.GenericTypeArguments[1],
					typeof(R)
					), 
					helper,
					SrcOrderItemProperty
					);
		}
		static MethodInfo MethodCreateConverter { get; } = typeof(MultipleRelationQueryConverterProvider)
			.GetMethodExt(
				nameof(CreateConverter), 
				BindingFlags.Static | BindingFlags.NonPublic, 
				typeof(IQueryResultBuildHelperCache),
				typeof(QueryMode),
				typeof(PropertyInfo)
			).IsNotNull();

		IQueryResultBuildHelperCache QueryResultBuildHelperCache { get; }
		public MultipleRelationQueryConverterProvider(IQueryResultBuildHelperCache QueryResultBuildHelperCache)
		{
			this.QueryResultBuildHelperCache = QueryResultBuildHelperCache;
		}
		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType,PropertyInfo DataModelProperty, Type EntityType,PropertyInfo EntityProperty,QueryMode QueryMode)
		{
			if (QueryMode == QueryMode.Summary)
				return null;
			if (DataModelProperty == null)
				return null;
			if (!DataModelProperty.PropertyType.IsGenericTypeOf(typeof(ICollection<>)))
				return null;

			if (!EntityProperty.PropertyType.IsGenericTypeOf(typeof(IEnumerable<>)))
				return null;

			var invesetPropertyAttr = DataModelProperty.GetCustomAttribute<InversePropertyAttribute>();
			if (invesetPropertyAttr == null)
				return null;

			var ModelItemType = DataModelProperty.PropertyType.GenericTypeArguments[0];
			var EntityItemType = EntityProperty.PropertyType.GenericTypeArguments[0];
			if (EntityItemType.IsConstType() || EntityItemType.GetCustomAttribute<EntityIdentAttribute>() != null)
				return null;

			var SrcOrderItemProperty = ModelItemType
				.AllPublicInstanceProperties()
				.FirstOrDefault(p => p.GetCustomAttribute<ItemOrderAttribute>() != null);

			return (IEntityPropertyQueryConverter)MethodCreateConverter.MakeGenericMethod(ModelItemType,EntityItemType).Invoke(
				null,
				new object[] { QueryResultBuildHelperCache, QueryMode, SrcOrderItemProperty }
				);

			/*
			select(
				s =>
				new {
					__Result=new EntityProperty { .
						
						X=s.users.select(mapper)
						Y=s.dsts.select(mapper)
						...},
					rootTemp=s.temp,
					xtemp= s.users.select(temp_mapper_with_id)
				}
				)

			*/


			//if ((DataModelProperty.GetCustomAttribute<ForeignKeyAttribute>() != null && DataModelProperty.GetCustomAttribute<KeyAttribute>() == null) ||
			//	 != null)
			//	return null;
			//if (!DataModelProperty.PropertyType.CanSimpleConvertTo(EntityProperty.PropertyType))
			//	return null;

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
			//return DefaultConverter.Instance;
		}
	}

}
