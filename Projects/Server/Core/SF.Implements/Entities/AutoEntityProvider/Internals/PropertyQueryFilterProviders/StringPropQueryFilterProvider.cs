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
