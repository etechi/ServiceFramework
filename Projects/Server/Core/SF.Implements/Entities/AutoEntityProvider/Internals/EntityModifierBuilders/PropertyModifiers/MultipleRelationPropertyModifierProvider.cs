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
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals.PropertyModifiers
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
			
			public EntityPropertyModifier(
				IEntityModifierCache EntityModifierCache, 
				PropertyInfo ForeignKeyProperty,
				Action<TChildModel, TParentKey> FuncSetParentKey,
				Func<TChildModel, TChildEntity, bool> FuncChildEqual
				)
			{
				this.FuncChildEqual = FuncChildEqual;
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
			public int ExecutePriority => 1000; //×îºóÖ´ÐÐ

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
				
				await set.MergeAsync(
					orgItems,
					Value,
					FuncChildEqual,
					async e =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitCreate(e,null);
						await create.Execute(Manager, ctx);
						FuncSetParentKey(ctx.Model, parentKey);
						return ctx.Model;
					},
					async (d, e) =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitUpdate(d, e, null);
						await update.Execute(Manager, ctx);
					},
					async d =>
					{
						var ctx = new EntityModifyContext<TChildEntity, TChildModel>();
						ctx.InitRemove(d, default(TChildEntity), null);
						await remove.Execute(Manager, ctx);
					}
					);
				return OrgValue;
			}
			public IEntityPropertyModifier Merge(IEntityPropertyModifier LowPriorityModifier) => this;
		}
		public IEntityPropertyModifier GetPropertyModifier(
			DataActionType ActionType, 
			Type EntityType, 
			PropertyInfo EntityProperty, 
			Type DataModelType, 
			PropertyInfo DataModelProperty
			)
		{
			if (ActionType != DataActionType.Update && ActionType != DataActionType.Create && ActionType!=DataActionType.Delete)
				return null;
			if(EntityProperty==null)
				return null;
			if (!EntityProperty.PropertyType.IsGenericTypeOf(typeof(IEnumerable<>)))
				return null;
			if(!DataModelProperty.PropertyType.IsGenericTypeOf(typeof(ICollection<>)) )
				return null;

			var childEntityType = EntityProperty.PropertyType.GenericTypeArguments[0];
			var childModelType = DataModelProperty.PropertyType.GenericTypeArguments[0];
			if (!childEntityType.IsClass || !childModelType.IsClass)
				return null;

			var inversePropertyAttr = DataModelProperty.GetCustomAttribute<InversePropertyAttribute>();
			if (inversePropertyAttr == null)
				return null;

			var ForeignProperty = childModelType.GetProperty(inversePropertyAttr.Property);
			if (ForeignProperty == null)
				return null;

			if (ForeignProperty.PropertyType != DataModelType)
				return null;

			var ForeignKeyPropAttr = ForeignProperty.GetCustomAttribute<ForeignKeyAttribute>();
			if (ForeignKeyPropAttr == null)
				return null;

			var ForeignKeyProp = childModelType.GetProperty(ForeignKeyPropAttr.Name);
			if (ForeignKeyProp == null)
				return null;

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
				return null;

			if (childEntityKeys.Zip(childModelKeys, (l, r) => l.Name != r.Name || l.PropertyType != r.PropertyType).Any(r=>r))
				return null;


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
						.Compile(argChildModel, argChildEntity)
					);
		}
	}

}
