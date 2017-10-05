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
	public class CombineEntityModifier<TEntity, TDataModel> : IEntityModifier<TEntity, TDataModel>
		where TDataModel:class
	{
		public int Priority { get; }
		public IEntityModifier<TEntity, TDataModel>[] Modifiers { get; }
		public CombineEntityModifier(int Priority, IEntityModifier<TEntity, TDataModel>[] Modifiers )
		{
			this.Modifiers = Modifiers;
			this.Priority = Priority;

		}
		public async Task Execute(
			IDataSetEntityManager<TEntity, TDataModel> Manager, 
			IEntityModifyContext<TEntity, TDataModel> Context
			)
		{
			foreach (var m in Modifiers)
				await m.Execute(Manager, Context);
		}
	}
}
