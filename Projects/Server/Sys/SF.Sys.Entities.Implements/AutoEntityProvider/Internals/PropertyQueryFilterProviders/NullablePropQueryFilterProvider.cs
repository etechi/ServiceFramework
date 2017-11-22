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
	public class NullablePropQueryFilterProvider : SinglePropQueryFilterProvider
	{

		class Filter<T> : PropQueryFilter<T?> where T:struct
		{
			public override int Priority => 10;

			public Filter(PropertyInfo Property) : base(Property)
			{
			}

			public override Expression OnGetFilterExpression(Expression prop, T? value)
			{
				return ContextQueryableFilters.GetFilterExpression(value, prop);
			}
		}

		public override int Priority => 1000;


		protected override bool MatchType(Type DataValueType, Type PropValueType)
		{
			if (!PropValueType.IsGenericTypeOf(typeof(Nullable<>)))
				return false;
			if (!DataValueType.IsValueType)
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
