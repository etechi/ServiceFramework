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
	public class StringPropQueryFilterProvider : SinglePropQueryFilterProvider
	{
		public override int Priority => 100;

		class Filter : PropQueryFilter<string>
		{
			bool UseContains { get; }
			public Filter(PropertyInfo Property, bool UseContains) : base(Property)
			{
				this.UseContains = UseContains;
			}

			public override Expression OnGetFilterExpression(Expression prop, string value)
			{
				if(UseContains)
					return ContextQueryableFilters.GetContainFilterExpression(value, prop);
				else
					return ContextQueryableFilters.GetStringFilterExpression(value, prop);
			}
		}

		protected override IPropertyQueryFilter CreateFilter(PropertyInfo dataProp, PropertyInfo queryProp)
		{
			return new Filter(dataProp, queryProp.GetCustomAttribute<StringContainsAttribute>() != null);
		}
		protected override bool MatchType(Type DataValueType, Type PropValueType)
		{
			return DataValueType == typeof(string) && PropValueType==typeof(string);
		}

		
	}
}
