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
using System.Reflection;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using SF.Sys.Linq.Expressions;
using SF.Sys.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyQueryConveters
{
	public class SingleRelationQueryConverterProvider : IEntityPropertyQueryConverterProvider
	{
		public int Priority => 0;
			
		class SingleRelationConverter<E,T,R> : IEntityPropertyQueryConverterAsync<T,R> 
			where T:class
			where R:class
		{
			public Type TempFieldType => typeof(T);
			IQueryResultBuildHelper<E, T, R> QueryResultBuildHelper { get; }
			public SingleRelationConverter(IQueryResultBuildHelper<E, R> QueryResultBuildHelper)
			{
				QueryResultBuildHelper = (IQueryResultBuildHelper < E, T, R > )QueryResultBuildHelper;
			}
			public Expression SourceToDestOrTemp(Expression src,int Level, PropertyInfo srcProp,PropertyInfo dstProp)
			{
				if (Level == 3)
					return null;
				return QueryResultBuildHelper.BuildEntityMapper(src.GetMember(srcProp),Level+1);
			}

			public async Task<R> TempToDest(object src, T value)
			{
				if (value == null) return null;
				var re= await QueryResultBuildHelper.ResultMapper(new[] { value });
				return re[0];
			}
		}
		static IEntityPropertyQueryConverter CreateConverter<E,R>(IQueryResultBuildHelperCache QueryResultBuildHelperCache, QueryMode QueryMode)
		{
			var helper = QueryResultBuildHelperCache.GetHelper<E, R>(QueryMode);
			if (helper == null)
				return null;

			var helperWithTemp = helper.GetType().AllInterfaces().FirstOrDefault(i => i.IsGenericTypeOf(typeof(IQueryResultBuildHelper<,,>)));

			return (IEntityPropertyQueryConverter)Activator.CreateInstance(
				typeof(SingleRelationConverter<,,>).MakeGenericType(
					typeof(E),
					helperWithTemp.GenericTypeArguments[1],
					typeof(R)
					));
		}
		static MethodInfo MethodCreateConverter { get; } = typeof(MultipleRelationQueryConverterProvider)
			.GetMethodExt(
				nameof(CreateConverter), 
				BindingFlags.Static | BindingFlags.NonPublic, 
				typeof(QueryResultBuildHelperCache)
			);

		IQueryResultBuildHelperCache QueryResultBuildHelperCache { get; }
		public SingleRelationQueryConverterProvider(IQueryResultBuildHelperCache QueryResultBuildHelperCache)
		{
			this.QueryResultBuildHelperCache = QueryResultBuildHelperCache;
		}
		public IEntityPropertyQueryConverter GetPropertyConverter(Type DataModelType,PropertyInfo DataModelProperty, Type EntityType,PropertyInfo EntityProperty,QueryMode QueryMode)
		{
			if (QueryMode == QueryMode.Summary)
				return null;
			if (DataModelProperty == null)
				return null;

			if (!DataModelProperty.PropertyType.IsConstType())
				return null;

			if (!EntityProperty.PropertyType.IsConstType())
				return null;

			var foreignKeyAttr = DataModelProperty.GetCustomAttribute<ForeignKeyAttribute>();
			if (foreignKeyAttr == null)
				return null;
			
			return (IEntityPropertyQueryConverter)MethodCreateConverter.MakeGenericMethod(DataModelProperty.PropertyType, EntityProperty.PropertyType).Invoke(
				null,
				new object[] { QueryResultBuildHelperCache, QueryMode }
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
