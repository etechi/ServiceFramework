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
using System.Linq;

namespace SF.Entities.AutoEntityProvider.Internals.PropertyModifiers
{
	public class CollectionPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		IEntityModifierCache EntityModifierCache { get; }
		public CollectionPropertyModifierProvider(IEntityModifierCache EntityModifierCache)
		{
			this.EntityModifierCache = EntityModifierCache;
		}
		class EntityPropertyModifier<E,D> : IAsyncEntityPropertyModifier<IEnumerable<E>, ICollection<D>>
			where D:class
		{
			IEntityModifierCache EntityModifierCache { get; }
			IEntityModifier<E, D> CreateModifier;
			IEntityModifier<E, D> UpdateModifier;

			public EntityPropertyModifier(IEntityModifierCache EntityModifierCache)
			{
				this.EntityModifierCache = EntityModifierCache;
			}
			public int Priority => 0;
			public async Task<ICollection<D>> Execute(IDataSetEntityManager Manager, IEntityModifyContext Context, ICollection<D> OrgValue, IEnumerable<E> Value)
			{
				if (Value == null)
					return OrgValue;
				switch (Context.Action)
				{
					case ModifyAction.Create:
						CreateModifier = CreateModifier ?? EntityModifierCache.GetEntityModifier<E, D>(DataActionType.Create);
						break;
					case ModifyAction.Update:
						 UpdateModifier = UpdateModifier ?? EntityModifierCache.GetEntityModifier<E, D>(DataActionType.Create);
						break;
					default:
						throw new NotSupportedException();
				}
				var orgItems = await Manager.DataContext.Set<D>().AsQueryable(false).Where(d=>true).ToArrayAsync();
				await Manager.DataContext.Set<D>().MergeAsync(
					orgItems,
					Value,
					(d, e) => true,
					async e =>
					{
						var mgr = Manager.ServiceProvider.Resolve<IDataSetEntityManager<E, D>>();
						var ctx = new EntityModifyContext<E, D>();
						mgr.InitCreateContext(ctx,e,null);
						await CreateModifier.Execute(mgr, ctx);
						return ctx.Model;
					},
					async (d, e) =>
					{
						var mgr = Manager.ServiceProvider.Resolve<IDataSetEntityManager<E, D>>();
						var ctx = new EntityModifyContext<E, D>();
						mgr.InitUpdateContext(ctx, e,d, null);
						await UpdateModifier.Execute(mgr, ctx);
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
			if (ActionType != DataActionType.Update && ActionType != DataActionType.Create)
				return null;
			if(EntityProperty==null)
				return null;
			if (!EntityProperty.PropertyType.IsGenericTypeOf(typeof(IEnumerable<>)))
				return null;
			if(!DataModelProperty.PropertyType.IsGenericTypeOf(typeof(ICollection<>)) )
				return null;
			var entityType = EntityProperty.PropertyType.GenericTypeArguments[0];
			var dataType = DataModelProperty.PropertyType.GenericTypeArguments[0];
			if (!entityType.IsClass || !dataType.IsClass)
				return null;


			return (IEntityPropertyModifier)Activator.CreateInstance(
				typeof(EntityPropertyModifier<,>).MakeGenericType(
					entityType,
					dataType
					),
					EntityModifierCache);
		}
	}

}
