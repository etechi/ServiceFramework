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
	public class ServiceScopeQueryFilterProvider : IQueryFilterProvider
	{
		class ServiceScopeQueryFilter<TDataModel, TQueryArgument> : IQueryFilter<TDataModel, TQueryArgument> 
		{
			public int Priority => 0;
			static ParameterExpression ArgModel { get; }=Expression.Parameter(typeof(TDataModel), "model");
			Expression PropertyGetter { get; }
			bool IsNullable { get; }
			public ServiceScopeQueryFilter(PropertyInfo prop)
			{
				PropertyGetter = Expression.Property(ArgModel, prop);
				IsNullable = prop.PropertyType.IsGenericTypeOf(typeof(Nullable<>));
			}
			public IContextQueryable<TDataModel> Filter(
				IContextQueryable<TDataModel> Query,
				IEntityServiceContext ServiceContext,
				TQueryArgument Arg
				)
			{
				var scopeid = ServiceContext.ServiceInstanceDescroptor.DataScopeId;
				if (!IsNullable && !scopeid.HasValue)
					return Query;
				return Query.Where(Expression.Lambda<Func<TDataModel,bool>>(
					Expression.Equal(
						PropertyGetter,
						 Expression.Constant(IsNullable ? scopeid:scopeid.Value,PropertyGetter.Type)
						),
					ArgModel
					)
				);
			}
		}


		public IQueryFilter<TDataModel, TQueryArgument> GetFilter<TDataModel, TQueryArgument>()
		{
			var prop = typeof(TDataModel).AllPublicInstanceProperties().FirstOrDefault(p => p.GetCustomAttribute<ServiceScopeIdAttribute>() != null);
			if (prop == null)
				return null;
			return new ServiceScopeQueryFilter<TDataModel, TQueryArgument>(prop);

		}
	}
}
