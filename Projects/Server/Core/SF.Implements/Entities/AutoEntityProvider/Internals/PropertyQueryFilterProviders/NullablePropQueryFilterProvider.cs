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
