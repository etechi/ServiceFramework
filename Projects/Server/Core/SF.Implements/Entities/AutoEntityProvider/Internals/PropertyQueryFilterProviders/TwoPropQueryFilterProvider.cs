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
	public abstract class TwoPropQueryFilterProvider: IPropertyQueryFilterProvider
	{

		protected abstract class PropQueryFilter<T> : IPropertyQueryFilter<T>
		{
			public int Priority => 100;

			public PropertyInfo Property1 { get; }
			public PropertyInfo Property2 { get; }
			public SinglePropQueryFilterProvider Provider { get; }

			public PropQueryFilter(PropertyInfo Property1, PropertyInfo Property2)
			{
				this.Property1 = Property1;
				this.Property2 = Property2;
			}
			public Expression GetFilterExpression(Expression obj, T value)
				=> OnGetFilterExpression(
					Expression.Property(obj, Property1),
					Expression.Property(obj, Property2),
					value
					);

			public abstract Expression OnGetFilterExpression(Expression prop1, Expression prop2, T value);
		}
		public abstract int Priority { get; }
		protected abstract bool MatchType(Type DataProp1Type, Type DataProp2Type, Type PropValueType);
		protected abstract IPropertyQueryFilter CreateFilter(PropertyInfo dataProp1, PropertyInfo dataProp2, PropertyInfo queryProp);
		public virtual (PropertyInfo,PropertyInfo) FindDataProperty(Type DataType, PropertyInfo queryProp)
		{
			return (
				DataType.GetProperty("Begin" + queryProp.Name),
				DataType.GetProperty("End" + queryProp.Name)
				);
		}
		public IPropertyQueryFilter GetFilter<TDataModel, TQueryArgument>(PropertyInfo queryProp)
		{
			var (p1,p2)= FindDataProperty(typeof(TDataModel),queryProp);
			if (p1 == null || p2==null)
				return null;
			if (!MatchType(p1.PropertyType,p2.PropertyType,queryProp.PropertyType))
				return null;
			return CreateFilter(p1, p2, queryProp);
		}
	}

	
}
