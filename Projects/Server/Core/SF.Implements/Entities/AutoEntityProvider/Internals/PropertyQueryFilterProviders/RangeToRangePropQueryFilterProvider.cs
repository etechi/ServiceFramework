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
	public class RangeToRangePropQueryFilterProvider : TwoPropQueryFilterProvider
	{
		class Filter<T> : PropQueryFilter<QueryRange<T>> where T: struct, IComparable<T>
		{
			public Filter(PropertyInfo Property1, PropertyInfo Property2) : base(Property1, Property2)
			{
			}

			public override Expression OnGetFilterExpression(Expression prop1, Expression prop2, QueryRange<T> value)
			{
				return ContextQueryableFilters.GetFilterExpression(value, prop1,prop2);
			}
		}

		public override int Priority => 1000;

		protected override bool MatchType(Type DataValueType1, Type DataValueType2, Type PropValueType)
		{
			if (!PropValueType.IsGenericTypeOf(typeof(QueryRange<>)))
				return false;
			if (DataValueType1 != PropValueType.GenericTypeArguments[0] )
				return false;
			if (DataValueType2 != PropValueType.GenericTypeArguments[0])
				return false;
			return true;
		}

		protected override IPropertyQueryFilter CreateFilter(PropertyInfo dataProp1, PropertyInfo dataProp2, PropertyInfo queryProp)
		{
			return (IPropertyQueryFilter )Activator.CreateInstance(
				typeof(Filter<>).MakeGenericType(dataProp1.PropertyType), 
				dataProp1,
				dataProp2
				);
		}
	}
}
