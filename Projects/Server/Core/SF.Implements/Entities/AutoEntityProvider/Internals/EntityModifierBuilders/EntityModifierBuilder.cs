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

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class EntityModifierBuilder : IEntityModifierBuilder
	{
		IEntityModifierProvider[] EntityModifierProviders { get; }

		public EntityModifierBuilder(IEnumerable<IEntityModifierProvider> EntityModifierProviders)
		{
			this.EntityModifierProviders = EntityModifierProviders.ToArray();
		}
		public IEntityModifier<TEntity, TDataModel> GetEntityModifier<TEntity,TDataModel>(DataActionType Type)
			where TDataModel:class
		{
			return new CombineEntityModifier<TEntity, TDataModel>(
				0,
				EntityModifierProviders
				.Select(p => p.GetEntityModifier<TEntity, TDataModel>(Type))
				.OrderBy(m => m.Priority)
				.ToArray()
				);
		}

	}
}
