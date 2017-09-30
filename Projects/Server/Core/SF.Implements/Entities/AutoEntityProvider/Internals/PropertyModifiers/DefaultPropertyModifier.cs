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
	public class DefaultPropertyModifierProvider : IEntityPropertyModifierProvider
	{
		public DefaultPropertyModifierProvider()
		{
		}
		class EntityPropertyModifier<T> : IEntityPropertyModifier<T, T>
		{
			public int Priority => 0;
			public T Execute(IDataSetEntityManager Manager, IEntityModifyContext Context, T Value)
			{
				return Value;
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
			if (ActionType != DataActionType.Update)
				return null;
			if(EntityProperty==null)
				return null;
			if (EntityProperty.PropertyType != DataModelProperty.PropertyType)
				return null;
			return (IEntityPropertyModifier)Activator.CreateInstance(typeof(EntityPropertyModifier<>).MakeGenericType(EntityProperty.PropertyType));
		}
	}

}
