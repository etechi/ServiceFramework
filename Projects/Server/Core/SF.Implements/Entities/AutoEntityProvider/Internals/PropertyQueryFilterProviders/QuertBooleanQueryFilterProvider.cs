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
	public class QueryBooleanPropQueryFilterProvider : SinglePropQueryFilterProvider
	{
		public override int Priority => 0;

		class Filter : PropQueryFilter<QueryableBoolean?>
		{
			public Filter(PropertyInfo Property) : base(Property)
			{
			}

			public override Expression OnGetFilterExpression(Expression prop, QueryableBoolean? value)
			{
				return ContextQueryableFilters.GetFilterExpression(value, prop);
			}
		}

		protected override IPropertyQueryFilter CreateFilter(PropertyInfo dataProp, PropertyInfo queryProp)
		{
			return new Filter(dataProp);
		}
		protected override bool MatchType(Type DataValueType, Type PropValueType)
		{
			return DataValueType == typeof(QueryableBoolean) && PropValueType==typeof(QueryableBoolean?);
		}

		
	}
}
