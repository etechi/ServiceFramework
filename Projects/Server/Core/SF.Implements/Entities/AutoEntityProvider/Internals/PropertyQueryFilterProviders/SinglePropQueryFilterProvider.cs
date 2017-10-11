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
using System.Linq.TypeExpressions;

namespace SF.Entities.AutoEntityProvider.Internals.QueryFilterProviders
{
	public abstract class SinglePropQueryFilterProvider: IPropertyQueryFilterProvider
	{

		protected abstract class PropQueryFilter<T> : IPropertyQueryFilter<T>
		{
			public virtual int Priority => 100;

			public PropertyInfo Property { get; }
			public SinglePropQueryFilterProvider Provider { get; }

			public PropQueryFilter(PropertyInfo Property)
			{
				this.Property = Property;
			}
			public Expression GetFilterExpression(Expression obj, T value)
				=> OnGetFilterExpression(Expression.Property(obj, Property), value);

			public abstract Expression OnGetFilterExpression(Expression prop, T value);
		}
		public abstract int Priority { get; }
		protected abstract bool MatchType(Type DataValueType,Type PropValueType);
		protected abstract IPropertyQueryFilter CreateFilter(PropertyInfo dataProp, PropertyInfo queryProp);
		public IPropertyQueryFilter GetFilter<TDataModel, TQueryArgument>(PropertyInfo queryProp)
		{
			var dataProp = typeof(TDataModel).GetProperty(queryProp.Name);
			if (dataProp == null)
				return null;
			if (!MatchType(dataProp.PropertyType,queryProp.PropertyType))
				return null;
			return CreateFilter(dataProp, queryProp);
		}
	}

	
}
