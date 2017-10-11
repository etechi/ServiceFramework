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
	public class RangePropQueryFilterProvider : SinglePropQueryFilterProvider
	{
		class Filter<T> : PropQueryFilter<QueryRange<T>> where T: struct, IComparable<T>
		{
			public Filter(PropertyInfo Property) : base(Property)
			{
			}

			public override Expression OnGetFilterExpression(Expression prop, QueryRange<T> value)
			{
				return ContextQueryableFilters.GetFilterExpression(value, prop);
			}
		}

		public override int Priority => 1000;


		protected override bool MatchType(Type DataValueType, Type PropValueType)
		{
			if (!PropValueType.IsGenericTypeOf(typeof(QueryRange<>)))
				return false;
			if (DataValueType != PropValueType.GenericTypeArguments[0])
				return false;
			return true;
		}

		protected override IPropertyQueryFilter CreateFilter(PropertyInfo dataProp, PropertyInfo queryProp)
		{
			return (IPropertyQueryFilter )Activator.CreateInstance(
				typeof(Filter<>).MakeGenericType(dataProp.PropertyType), 
				dataProp
				);
		}
	}
}
