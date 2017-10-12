#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
	public class DateTimeRangeToRangePropQueryFilterProvider : TwoPropQueryFilterProvider
	{
		class Filter : PropQueryFilter<QueryRange<DateTime>>
		{
			public Filter(PropertyInfo Property1, PropertyInfo Property2) : base(Property1, Property2)
			{
			}

			public override Expression OnGetFilterExpression(Expression prop1, Expression prop2, QueryRange<DateTime> value)
			{
				return ContextQueryableFilters.GetFilterExpression(value, prop1,prop2, DateTime.Now.Date.AddDays(-31));
			}
		}

		public override int Priority => 1000;

		protected override bool MatchType(Type DataValueType1, Type DataValueType2, Type PropValueType)
		{
			if (PropValueType!=typeof(QueryRange<DateTime>))
				return false;
			if (DataValueType1 != typeof(DateTime))
				return false;
			if (DataValueType2 != typeof(DateTime))
				return false;
			return true;
		}

		protected override IPropertyQueryFilter CreateFilter(PropertyInfo dataProp1, PropertyInfo dataProp2, PropertyInfo queryProp)
		{
			return new Filter(dataProp1,dataProp2);
		}
	}
}
