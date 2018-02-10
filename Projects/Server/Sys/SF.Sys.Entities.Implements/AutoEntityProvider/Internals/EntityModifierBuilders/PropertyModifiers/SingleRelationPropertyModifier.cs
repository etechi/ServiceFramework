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
using SF.Sys.Linq.Expressions;
using SF.Sys.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.PropertyModifiers
{
	public class SingleRelationPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		IEntityModifierCache EntityModifierCache { get; }
		
		public SingleRelationPropertyModifierProvider(IEntityModifierCache EntityModifierCache)
		{
			this.EntityModifierCache = EntityModifierCache;
		}
		class EntityPropertyModifier<TParentKey, TEntity,TModel, TChildEntity,TChildModel> : 
			IAsyncEntityPropertyModifier<TChildEntity, TChildModel>
			where TChildModel:class,new()
			where TModel:class
			where TParentKey:IEquatable<TParentKey>
		{
			Lazy<(IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>, IEntityModifier<TChildEntity, TChildModel>)> LazyModifiers { get; }
			PropertyInfo ForeignKeyProperty { get; }
			Action<TChildModel,TParentKey> FuncSetParentKey { get; }
			
			public EntityPropertyModifier(
				IEntityModifierCache EntityModifierCache, 
				PropertyInfo ForeignKeyProperty,
				Action<TChildModel, TParentKey> FuncSetParentKey
				)
			{
				this.FuncSetParentKey = FuncSetParentKey;
				this.ForeignKeyProperty = ForeignKeyProperty;
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

			public async Task<TChildModel> Execute(IEntityServiceContext Manager, IEntityModifyContext Context, TChildModel OrgValue, TChildEntity Value)
			{
				if (Value.IsDefault() && Context.Action != ModifyAction.Delete)
					return OrgValue;
				var (create, update, remove) = LazyModifiers.Value;
				var set = Context.DataContext.Set<TChildModel>();

				var parentCtx = (IEntityModifyContext<TEntity, TModel>)Context;
				var parentKey = Entity<TModel>.GetKey<ObjectKey<TParentKey>>(parentCtx.Model).Id;

				var arg = Expression.Parameter(typeof(TChildModel));
				var orgItem = await Context.DataContext.Set<TChildModel>().AsQueryable(false).Where(
					Expression.Lambda<Func<TChildModel, bool>>(
						Expression.Equal(
							Expression.Property(arg, ForeignKeyProperty),
							Expression.Constant(parentKey, ForeignKeyProperty.PropertyType)
						),
						arg
					)
					).SingleOrDefaultAsync();

				var merger = new Merger
				{
					Modifier = this,
					ModifyContext = Context,
					ServiceContext = Manager,
					ParentKey = parentKey,
					ChildModels= orgItem == null ? Enumerable.Empty<TChildModel>() : EnumerableEx.From(orgItem ),
					ChildEditables= Value.IsDefault() ? Enumerable.Empty<TChildEntity>() : EnumerableEx.From(Value )
				};
				var handler = Context.ModifyHandlerProvider?.FindMergeHandler<TChildEntity, TChildModel>();
				if (handler == null)
					return (await merger.Merge(null)).SingleOrDefault();
				else
					return (await handler.Merge(merger)).SingleOrDefault();
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;

			public class Merger : IEntityChildMerger<TChildEntity, TChildModel>
			{
				public EntityPropertyModifier<TParentKey, TEntity, TModel, TChildEntity, TChildModel> Modifier { get; set; }
				public IEntityServiceContext ServiceContext { get; set; }
				public IEntityModifyContext ModifyContext { get; set; }
				public TParentKey ParentKey { get; set; }
				public IEnumerable<TChildModel> ChildModels { get; set; }
				public IEnumerable<TChildEntity> ChildEditables { get; set; }
				public async Task<ICollection<TChildModel>> Merge(
					Func<IEntityModifyContext<TChildEntity, TChildModel>,ModifyAction,Task> ModifyHandler
				)
				{
					var set = ModifyContext.DataContext.Set<TChildModel>();
					var Value = ChildEditables.SingleOrDefault();
					var orgItem = ChildModels.SingleOrDefault();

					var (create, update, remove) = Modifier.LazyModifiers.Value;
					//删除
					if (ModifyContext.Action == ModifyAction.Delete)
					{
						if (orgItem != null)
						{
							var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
							ctx.InitRemove(ModifyContext.DataContext, orgItem, default(TChildEntity), null);
							if (ModifyHandler != null)
								await ModifyHandler(ctx,ModifyAction.Delete);
							await remove.Execute(ServiceContext, ctx);
							set.Remove(orgItem);
						}
						return new List<TChildModel>();
					}
					//新建
					else if (orgItem == null)
					{
						var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
						ctx.InitCreate(ModifyContext.DataContext, Value, null);

						if (ModifyHandler != null)
							await ModifyHandler(ctx, ModifyAction.Create);

						await create.Execute(ServiceContext, ctx);

						if (ModifyHandler != null)
							await ModifyHandler(ctx,ModifyAction.Update);
						await update.Execute(ServiceContext, ctx);

						Modifier.FuncSetParentKey(ctx.Model, ParentKey);
						return new List<TChildModel> { ctx.Model };
					}
					//编辑
					else
					{
						var ctx = ModifyContext.CreateChildModifyContext<TChildEntity, TChildModel>();
						ctx.InitUpdate(ModifyContext.DataContext, orgItem, Value, null);

						if (ModifyHandler != null)
							await ModifyHandler(ctx,ModifyAction.Update);

						await update.Execute(ServiceContext, ctx);
						return ChildModels.ToList();
					}
				}
			}
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{
			if (ActionType != DataActionType.Update &&  ActionType!=DataActionType.Delete)
				return null;
			if(EntityProperty==null)
				return null;

			var foreignKeyAttr = DataModelProperty.GetCustomAttribute<ForeignKeyAttribute>();
			if (foreignKeyAttr == null)
				return null;

			var childModelType = DataModelProperty.PropertyType;
			var childModelKeys = (IReadOnlyList<PropertyInfo>)typeof(Entity<>)
					.MakeGenericType(childModelType)
					.GetProperty(nameof(Entity<string>.KeyProperties))
					.GetValue(null);
			if(childModelKeys.Count!=1)
				return null;

			var childEntityType = EntityProperty.PropertyType;
			var childEntityKeys = (IReadOnlyList<PropertyInfo>)typeof(Entity<>)
				.MakeGenericType(childEntityType)
				.GetProperty(nameof(Entity<string>.KeyProperties))
				.GetValue(null);
			if (childEntityKeys.Count != 1)
				return null;
			if (childEntityKeys[0].Name != childModelKeys[0].Name || childEntityKeys[0].PropertyType != childModelKeys[0].PropertyType)
				return null;
			var ForeignKeyProp = childModelKeys[0];

			var childModelForeignKeyAttr = ForeignKeyProp.PropertyType.GetCustomAttribute<ForeignKeyAttribute>();
			if (childModelForeignKeyAttr == null)
				return null;
			var childModelForeignProp = childModelType.GetProperty(childModelForeignKeyAttr.Name);
			if (childModelForeignProp == null)
				return null;
			if (childModelForeignProp.PropertyType != DataModelType)
				return null;


			var argChildModel = Expression.Parameter(childModelType);
			var argParentKey = Expression.Parameter(ForeignKeyProp.PropertyType);
			var argChildEntity = Expression.Parameter(childEntityType);

		
			

			if (childModelKeys.Count != childEntityKeys.Count)
				return null;
			if (childModelKeys.Count > 2)
				return null;

			if (childEntityKeys.Zip(childModelKeys, (l, r) => l.Name != r.Name || l.PropertyType != r.PropertyType).Any(r=>r))
				return null;

			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(EntityPropertyModifier<,,,,>).MakeGenericType(
					childModelKeys[0].PropertyType,
					EntityType,
					DataModelType,
					childEntityType,
					childModelType
					),
					EntityModifierCache,
					ForeignKeyProp,
					argChildModel.SetProperty(ForeignKeyProp,argParentKey)
						.Compile(argChildModel,argParentKey)
					);
		}
	}

}
