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
using System.ComponentModel.DataAnnotations;
using SF.Sys.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyModifiers
{
	
	public class MultipleRelationPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		IEntityModifierCache EntityModifierCache { get; }
		
		public MultipleRelationPropertyModifierProvider(IEntityModifierCache EntityModifierCache)
		{
			this.EntityModifierCache = EntityModifierCache;
		}
		class EntityPropertyModifier<TParentKey, TEntity,TModel,TChildKey, TChildEntity,TChildModel> : 
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
				var merger = new Merger
				{
					Modifier = this,
					ModifyContext = Context,
					ServiceContext = Manager,
					ParentKey = parentKey,
					ChildEditables=Value,
					ChildModels=orgItems
				};
				var handler = Context.ModifyHandlerProvider?.FindMergeHandler<TChildEntity, TChildModel>();
				if (handler == null)
					return await merger.Merge(
						null
						);
				else
					return await handler.Merge(merger);
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;

			public class Merger : IEntityChildMerger<TChildEntity, TChildModel> 
			{
				public EntityPropertyModifier<TParentKey, TEntity, TModel, TChildKey, TChildEntity, TChildModel> Modifier { get; set; }
				public IEntityServiceContext ServiceContext{ get; set; }
				public IEntityModifyContext ModifyContext{get; set; }
				public TParentKey ParentKey { get; set; }
				public IEnumerable<TChildModel> ChildModels { get; set; }
				public IEnumerable<TChildEntity> ChildEditables { get; set; }
				public async Task<ICollection<TChildModel>> Merge(
					Func<IEntityModifyContext<TChildEntity, TChildModel>,ModifyAction,Task> ModifyHandler
				)
				{
					var newItems = ChildEditables?.Select((v, i) => (v, i))?.ToArray() ?? Array.Empty<(TChildEntity v, int i)>();
					var childMeta = ServiceContext.EntityMetadataCollection.FindByEntityType(typeof(TChildEntity));
					var set = ModifyContext.DataContext.Set<TChildModel>();
					var (create, update, remove) = Modifier.LazyModifiers.Value;

					var OrgValue = await set.MergeAsync(
						ChildModels,
						newItems,
						(d, e) => 
							Modifier.FuncChildEqual(d, e.v),
						async e =>
						{
							var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
							ctx.InitCreate(ModifyContext.DataContext, e.v, null);
							if (ModifyHandler != null)
								await ModifyHandler(ctx,ModifyAction.Create);

							await create.Execute(ServiceContext, ctx);

							if (ModifyHandler != null)
								await ModifyHandler(ctx, ModifyAction.Update);

							await update.Execute(ServiceContext, ctx);

							Modifier.FuncSetParentKey(ctx.Model, ParentKey);
							Modifier.FuncSetItemOrder?.Invoke(ctx.Model, e.i);
							ServiceContext.PostChangedEvents(
								ModifyContext.DataContext, 
								Entity<TChildModel>.GetKey<TChildKey>(ctx.Model),
								ctx.Editable, 
								DataActionType.Create, 
								childMeta
								);
							return ctx.Model;
						},
						async (d, e) =>
						{
							var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
							ctx.InitUpdate(ModifyContext.DataContext, d, e.v, null);
							Modifier.FuncSetItemOrder?.Invoke(ctx.Model, e.i);
							if (ModifyHandler != null)
								await ModifyHandler(ctx, ModifyAction.Update);

							await update.Execute(ServiceContext, ctx);
							ServiceContext.PostChangedEvents(
								ModifyContext.DataContext,
								Entity<TChildModel>.GetKey<TChildKey>(ctx.Model), 
								ctx.Editable, 
								DataActionType.Update, 
								childMeta
								);
						},
						async d =>
						{
							var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
							ctx.InitRemove(ModifyContext.DataContext, d, Entity<TChildModel>.GetKeyWithoutValidate<TChildEntity>(d), null);
							if (ModifyHandler != null)
								await ModifyHandler(ctx, ModifyAction.Delete);

							await remove.Execute(ServiceContext, ctx);
							ServiceContext.PostChangedEvents(
								ModifyContext.DataContext,
								Entity<TChildModel>.GetKey<TChildKey>(ctx.Model),
								ctx.Editable, 
								DataActionType.Delete, 
								childMeta
								);
						}
						);
					return OrgValue;
				}
			}
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

			var childKeyProps = (from p in childModelType.AllPublicInstanceProperties().Select((prop, idx) => (prop, idx))
								 where p.prop.IsDefined(typeof(KeyAttribute))
								 let col = p.prop.GetCustomAttribute<ColumnAttribute>()?.Order ?? p.idx
								 orderby col
								 select p.prop
							   ).ToArray();
			if (childKeyProps.Length < 1 && childKeyProps.Length > 4)
				throw new NotSupportedException($"子项主键个数必须是1-3个，类型{childModelType.FullName}的主键为:{childKeyProps.Select(p => p.PropertyType.ShortName() + " " + p.Name).Join(",")}");
			var childKeyType = ObjectKey.CreateKeyType(childKeyProps.Select(p => p.PropertyType).ToArray());

			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(EntityPropertyModifier<,,,,,>).MakeGenericType(
					ForeignKeyProp.PropertyType,
					EntityType,
					DataModelType,
					childKeyType,
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
