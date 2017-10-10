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
	public abstract class SinglePropQueryFilterProvider<T> : IPropertyQueryFilterProvider
	{
		class PropQueryFilter : IPropertyQueryFilter<T>
		{
			public int Priority => 0;

			public PropertyInfo Property { get; }
			public SinglePropQueryFilterProvider<T> Provider { get; }

			public PropQueryFilter(PropertyInfo Property, SinglePropQueryFilterProvider<T> Provider)
			{
				this.Property = Property;
				this.Provider = Provider;
			}
			public Expression GetFilterExpression(Expression obj, T value)
			{
				return Provider.GetFilterExpression(Expression.Property(obj, Property), value);
			}
		}
		protected abstract Expression GetFilterExpression(Expression prop, T value);
		protected abstract bool MatchType(Type DataValueType);
		public IPropertyQueryFilter GetFilter<TDataModel, TQueryArgument>(PropertyInfo queryProp)
		{
			if (queryProp.PropertyType != typeof(T))
				return null;
			var dataProp = typeof(TDataModel).GetProperty(queryProp.Name);
			if (dataProp == null)
				return null;
			if (!MatchType(dataProp.PropertyType))
				return null;
			return new PropQueryFilter(dataProp, this);
		}
	}

	public class StringPropQueryFilterProvider : SinglePropQueryFilterProvider<string>
	{
		protected override Expression GetFilterExpression(Expression prop, string value)
		{
			return ContextQueryableFilters.GetStringFilterExpression(value, prop);
		}

		protected override bool MatchType(Type DataValueType)
		{
			return DataValueType == typeof(string);
		}
		
	}
}
