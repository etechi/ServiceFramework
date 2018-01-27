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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using SF.Sys.Reflection;
using SF.Sys.Data;
using SF.Sys.Linq.Expressions;
using SF.Sys.Annotations;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyModifiers
{
	public class MultipleRelationIdentPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		IEntityModifierCache EntityModifierCache { get; }
		class EmptyEntity {
			public static EmptyEntity Instance { get; } = new EmptyEntity();
		}

		public MultipleRelationIdentPropertyModifierProvider(IEntityModifierCache EntityModifierCache)
		{
			this.EntityModifierCache = EntityModifierCache;
		}
		class EntityPropertyModifier<TParentKey, TEntity,TModel, TChildEntityIdent,TChildModel> : 
			IAsyncEntityPropertyModifier<IEnumerable<TChildEntityIdent>, ICollection<TChildModel>>
			where TChildModel:class,new()
			where TModel:class
			where TParentKey:IEquatable<TParentKey>
		{
			Lazy<(IEntityModifier<EmptyEntity, TChildModel>, IEntityModifier<EmptyEntity, TChildModel>, IEntityModifier<EmptyEntity, TChildModel>)> LazyModifiers { get; }
			PropertyInfo ForeignKeyProperty { get; }
			Action<TChildModel,TParentKey> FuncSetParentKey { get; }
			Func<TChildModel,TChildEntityIdent,bool> FuncChildEqual { get; }
			Action<TChildModel,int> FuncSetItemOrder { get; }
			Action<TChildModel, TChildEntityIdent> FuncSetItemKey { get; }
			public EntityPropertyModifier(
				IEntityModifierCache EntityModifierCache, 
				PropertyInfo ForeignKeyProperty,
				Action<TChildModel, TParentKey> FuncSetParentKey,
				Func<TChildModel, TChildEntityIdent, bool> FuncChildEqual,
				Action<TChildModel,TChildEntityIdent> FuncSetItemKey,
				Action<TChildModel, int> FuncSetItemOrder
				)
			{
				this.FuncSetItemKey = FuncSetItemKey;
				this.FuncChildEqual = FuncChildEqual;
				this.FuncSetParentKey = FuncSetParentKey;
				this.ForeignKeyProperty = ForeignKeyProperty;
				this.FuncSetItemOrder = FuncSetItemOrder;
				LazyModifiers = new Lazy<(IEntityModifier<EmptyEntity, TChildModel>, IEntityModifier<EmptyEntity, TChildModel>, IEntityModifier<EmptyEntity, TChildModel>)>(
					() =>
					(
					EntityModifierCache.GetEntityModifier<EmptyEntity, TChildModel>(DataActionType.Create),
					EntityModifierCache.GetEntityModifier<EmptyEntity, TChildModel>(DataActionType.Update),
					EntityModifierCache.GetEntityModifier<EmptyEntity, TChildModel>(DataActionType.Delete)
					)
					);
			}
			public int MergePriority => 0;
			public int ExecutePriority => 1000; //最后执行
			public async Task<ICollection<TChildModel>> Execute(
				IEntityServiceContext Manager, 
				IEntityModifyContext Context, 
				ICollection<TChildModel> OrgValue, 
				IEnumerable<TChildEntityIdent> Value
				)
			{
				if (Value == null && Context.Action!=ModifyAction.Delete)
					return OrgValue;
				var (create,update,remove)= LazyModifiers.Value;
				var set = Context.DataContext.Set<TChildModel>();

				var parentCtx = (IEntityModifyContext<TEntity, TModel>)Context;
				var parentKey = Entity<TModel>.GetKey<ObjectKey<TParentKey>>(parentCtx.Model).Id;

				var arg = Expression.Parameter(typeof(TChildModel));
				var orgItems = await Context.DataContext.Set<TChildModel>().AsQueryable(false).Where(
					Expression.Lambda<Func<TChildModel,bool>>(
						Expression.Equal(
							Expression.Property(arg,ForeignKeyProperty),
							Expression.Constant(parentKey,ForeignKeyProperty.PropertyType)
						),
						arg
					)
					).ToArrayAsync();

				var newItems = Value.Select((v, i) => (v, i)).ToArray();		
				await set.MergeAsync(
					orgItems,
					newItems,
					(d,e)=>FuncChildEqual(d,e.v),
					async e =>
					{
						var ctx = new EntityModifyContext<EmptyEntity, TChildModel>();
						ctx.InitCreate(ctx.DataContext, EmptyEntity.Instance, null);
						await create.Execute(Manager, ctx);
						FuncSetItemKey(ctx.Model, e.v);
						FuncSetParentKey(ctx.Model, parentKey);
						FuncSetItemOrder?.Invoke(ctx.Model, e.i);
						return ctx.Model;
					},
					async (d, e) =>
					{
						var ctx = new EntityModifyContext<EmptyEntity, TChildModel>();
						ctx.InitUpdate(ctx.DataContext,d, EmptyEntity.Instance, null);
						FuncSetItemKey(ctx.Model, e.v);
						FuncSetItemOrder?.Invoke(ctx.Model, e.i);
						await update.Execute(Manager, ctx);
					},
					async d =>
					{
						var ctx = new EntityModifyContext<EmptyEntity, TChildModel>();
						ctx.InitRemove(ctx.DataContext,d, null, null);
						await remove.Execute(Manager, ctx);
					}
					);
				return OrgValue;
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;
		}

		static string Desc(PropertyInfo EntityProperty, PropertyInfo DataModelProperty)
			=> $"一对多关系ID修改支持{EntityProperty.DeclaringType.FullName}.{EntityProperty.Name}=>{DataModelProperty.DeclaringType.FullName}.{DataModelProperty.Name}";

		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{
			if (ActionType != DataActionType.Update  && ActionType!=DataActionType.Delete)
				return null;
			if(EntityProperty==null)
				return null;
			if (!EntityProperty.PropertyType.IsGenericTypeOf(typeof(IEnumerable<>)))
				return null;
			if(!DataModelProperty.PropertyType.IsGenericTypeOf(typeof(ICollection<>)) )
				return null;

			var childEntityIdentType = EntityProperty.PropertyType.GenericTypeArguments[0];
			if (!childEntityIdentType.IsConstType())
				return null;

			var childModelType = DataModelProperty.PropertyType.GenericTypeArguments[0];
			if ( !childModelType.IsClass)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象{childModelType.FullName}不是引用类型");

			var inversePropertyAttr = DataModelProperty.GetCustomAttribute<InversePropertyAttribute>();
			if (inversePropertyAttr == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的实体属性{DataModelProperty.Name}没有定义InverseProperty特性");

			var ForeignProperty = childModelType.GetProperty(inversePropertyAttr.Property);
			if (ForeignProperty == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象未定义外键属性{inversePropertyAttr.Property}");

			if (ForeignProperty.PropertyType != DataModelType)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象外键属性类型({ForeignProperty.PropertyType.FullName})和父对象类型不同");

			var ForeignKeyPropAttr = ForeignProperty.GetCustomAttribute<ForeignKeyAttribute>();
			if (ForeignKeyPropAttr == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象外键属性未定义外键字段特性(ForeignKeyAttribute)");

			var ForeignKeyProp = childModelType.GetProperty(ForeignKeyPropAttr.Name);
			if (ForeignKeyProp == null)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 中的数据对象中不到ForeignKeyAttribute指定的外键字段{ForeignKeyPropAttr.Name}");

			var argChildModel = Expression.Parameter(childModelType);
			var argParentKey = Expression.Parameter(ForeignKeyProp.PropertyType);
			var argChildIdent = Expression.Parameter(childEntityIdentType);
			
			var childModelKeys = ((IReadOnlyList<PropertyInfo>)typeof(Entity<>)
				.MakeGenericType(childModelType)
				.GetProperty(nameof(Entity<string>.KeyProperties))
				.GetValue(null)).Where(p => p.Name != ForeignKeyProp.Name).ToArray();


			if (childModelKeys.Length != 1)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 实体和数据对象的主键数目不同");

			if (childModelKeys[0].PropertyType!=childEntityIdentType)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 实体和数据对象的主键类型不同");


			Delegate SetIndex = null;
			var ItemOrderProp = childModelType.AllPublicInstanceProperties().FirstOrDefault(p => p.GetCustomAttribute<ItemOrderAttribute>() != null);
			if (ItemOrderProp != null)
			{
				if (ItemOrderProp.PropertyType != typeof(int))
					throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 排位字段类型必须为整形");
				var argIndex = Expression.Parameter(typeof(int));
				SetIndex = argChildModel.SetProperty(ItemOrderProp, argIndex).Compile(
					typeof(Action<,>).MakeGenericType(argChildModel.Type, typeof(int)),
					argChildModel,
					argIndex
					);
			}

			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(EntityPropertyModifier<,,,,>).MakeGenericType(
					ForeignKeyProp.PropertyType,
					EntityType,
					DataModelType,
					childEntityIdentType,
					childModelType
					),
					EntityModifierCache,
					ForeignKeyProp,
					argChildModel.SetProperty(ForeignKeyProp,argParentKey)
						.Compile(argChildModel,argParentKey),
					argChildModel.GetMember(childModelKeys[0]).Equal(argChildIdent)
						.Compile(argChildModel, argChildIdent),
					argChildModel.SetProperty(childModelKeys[0], argChildIdent).Compile(
						typeof(Action<,>).MakeGenericType(argChildModel.Type, childEntityIdentType),
						argChildModel,
						argChildIdent
						),
					SetIndex
					);
		}
	}

}
