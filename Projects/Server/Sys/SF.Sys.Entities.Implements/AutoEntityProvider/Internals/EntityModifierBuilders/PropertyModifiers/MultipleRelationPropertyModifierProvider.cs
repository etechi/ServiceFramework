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
using SF.Sys.Linq.Expressions;
using SF.Sys.Annotations;
using SF.Sys.Data;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyModifiers
{
	public class MultipleRelationPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		IEntityModifierCache EntityModifierCache { get; }
		
		public MultipleRelationPropertyModifierProvider(IEntityModifierCache EntityModifierCache)
		{
			this.EntityModifierCache = EntityModifierCache;
		}
		class EntityPropertyModifier<TParentKey, TEntity,TModel, TChildEntity,TChildModel> : 
			IAsyncEntityPropertyModifier<IEnumerable<TChildEntity>, ICollection<TChildModel>>
			where TChildModel:class,new()
			where TModel:class
			where TParentKey:IEquatable<TParentKey>
		{
			Lazy<(IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>)> LazyModifiers { get; }
			PropertyInfo ForeignKeyProperty { get; }
			Action<TChildModel,TParentKey> FuncSetParentKey { get; }
			Func<TChildModel,TChildEntity,bool> FuncChildEqual { get; }
			Action<TChildModel,int> FuncSetItemOrder { get; }

			public EntityPropertyModifier(
				IEntityModifierCache EntityModifierCache, 
				PropertyInfo ForeignKeyProperty,
				Action<TChildModel, TParentKey> FuncSetParentKey,
				Func<TChildModel, TChildEntity, bool> FuncChildEqual,
				Action<TChildModel, int> FuncSetItemOrder
				)
			{
				this.FuncChildEqual = FuncChildEqual;
				this.FuncSetParentKey = FuncSetParentKey;
				this.ForeignKeyProperty = ForeignKeyProperty;
				this.FuncSetItemOrder = FuncSetItemOrder;
				LazyModifiers = new Lazy<(IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>)>(
					() =>
					(
					EntityModifierCache.GetEntityModifier<TChildEntity, TChildModel>(DataActionType.Create),
					EntityModifierCache.GetEntityModifier<TChildEntity, TChildModel>(DataActionType.Update),
					EntityModifierCache.GetEntityModifier<TChildEntity, TChildModel>(DataActionType.Delete)
					)
					);
			}
			public int MergePriority => 0;
			public int ExecutePriority => 1000; //最后执行

			
			public async Task<ICollection<TChildModel>> Execute(IEntityServiceContext Manager, IEntityModifyContext Context, ICollection<TChildModel> OrgValue, IEnumerable<TChildEntity> Value)
			{
				if (Value == null && Context.Action!=ModifyAction.Delete)
					return OrgValue;
				var (create,update,remove)= LazyModifiers.Value;
				var set = Manager.DataContext.Set<TChildModel>();

				var parentCtx = (IEntityModifyContext<TEntity, TModel>)Context;
				var parentKey = Entity<TModel>.GetKey<ObjectKey<TParentKey>>(parentCtx.Model).Id;

				var arg = Expression.Parameter(typeof(TChildModel));
				var orgItems = await Manager.DataContext.Set<TChildModel>().AsQueryable(false).Where(
					Expression.Lambda<Func<TChildModel,bool>>(
						Expression.Equal(
							Expression.Property(arg,ForeignKeyProperty),
							Expression.Constant(parentKey,ForeignKeyProperty.PropertyType)
						),
						arg
					)
					).ToArrayAsync();

				var newItems = Value?.Select((v, i) => (v, i))?.ToArray() ?? Array.Empty<(TChildEntity v,int i)>();
				var childMeta = Manager.EntityMetadataCollection.FindByEntityType(typeof(TChildEntity));

				await set.MergeAsync(
					orgItems,
					newItems,
					(d,e)=>FuncChildEqual(d,e.v),
					async e =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitCreate(e.v,null);
						await create.Execute(Manager, ctx);
						await update.Execute(Manager, ctx);

						FuncSetParentKey(ctx.Model, parentKey);
						FuncSetItemOrder?.Invoke(ctx.Model, e.i);

						Manager.PostChangedEvents( ctx.Editable, DataActionType.Create, childMeta);
						return ctx.Model;
					},
					async (d, e) =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitUpdate(d, e.v, null);
						FuncSetItemOrder?.Invoke(ctx.Model, e.i);
						await update.Execute(Manager, ctx);
						Manager.PostChangedEvents(ctx.Editable, DataActionType.Update, childMeta);
					},
					async d =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitRemove(d, Entity<TChildModel>.GetKey<TChildEntity>(d), null);
						await remove.Execute(Manager, ctx);
						Manager.PostChangedEvents(ctx.Editable, DataActionType.Delete, childMeta);
					}
					);
				return OrgValue;
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;
		}

		static string Desc(PropertyInfo EntityProperty, PropertyInfo DataModelProperty)
			=> $"一对多关系修改支持{EntityProperty.DeclaringType.FullName}.{EntityProperty.Name}=>{DataModelProperty.DeclaringType.FullName}.{DataModelProperty.Name}";

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

			var childEntityType = EntityProperty.PropertyType.GenericTypeArguments[0];
			if (childEntityType.IsConstType())
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
			var argChildEntity = Expression.Parameter(childEntityType);
			
			var childModelKeys = ((IReadOnlyList<PropertyInfo>)typeof(Entity<>)
				.MakeGenericType(childModelType)
				.GetProperty(nameof(Entity<string>.KeyProperties))
				.GetValue(null)).Where(p => p.Name != ForeignKeyProp.Name).ToArray();

			var childEntityKeys = ((IReadOnlyList<PropertyInfo>)typeof(Entity<>)
				.MakeGenericType(childEntityType)
				.GetProperty(nameof(Entity<string>.KeyProperties))
				.GetValue(null)
				).Where(p => p.Name != ForeignKeyProp.Name).ToArray();

			if (childModelKeys.Length==0 || childModelKeys.Length != childEntityKeys.Length)
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 实体和数据对象的主键数目不同");

			if (childEntityKeys.Zip(childModelKeys, (l, r) => l.Name != r.Name || l.PropertyType != r.PropertyType).Any(r=>r))
				throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 实体和数据对象的主键名称或类型不同");


			Delegate SetIndex = null;
			var ItemOrderProp = childModelType.AllPublicInstanceProperties().FirstOrDefault(p => p.GetCustomAttribute<ItemOrderAttribute>() != null);
			if (ItemOrderProp != null)
			{
				if(ItemOrderProp.PropertyType!=typeof(int))
					throw new NotSupportedException($"{Desc(EntityProperty, DataModelProperty)} 排位字段类型必须为整形");
				var argIndex = Expression.Parameter(typeof(int));
				SetIndex = argChildModel.SetProperty(ItemOrderProp, argIndex).Compile(
					typeof(Action<,>).MakeGenericType(argChildModel.Type,typeof(int)),
					argChildModel, 
					argIndex
					);
			}

			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(EntityPropertyModifier<,,,,>).MakeGenericType(
					ForeignKeyProp.PropertyType,
					EntityType,
					DataModelType,
					childEntityType,
					childModelType
					),
					EntityModifierCache,
					ForeignKeyProp,
					argChildModel.SetProperty(ForeignKeyProp,argParentKey)
						.Compile(argChildModel,argParentKey),
					childModelKeys.Zip(
						childEntityKeys,
						(m,e)=>argChildModel.GetMember(m).Equal(argChildEntity.GetMember(e))
						)
						.Aggregate(
							(l,r)=>Expression.AndAlso(l,r)
							)
						.Compile(
							argChildModel, argChildEntity
							),
					SetIndex

					);
		}
	}

}
