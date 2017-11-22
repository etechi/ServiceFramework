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
using SF.Sys.Reflection;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.QueryFilterProviders
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
