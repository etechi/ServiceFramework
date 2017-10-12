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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Core.Logging;
using SF.Core.Times;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.Services.Tests;
using System.Linq.Expressions;
using System.Reflection;
namespace SF.Entities.Tests.EntityQueryArgumentGenerators
{
	class DefaultEntityQueryArgumentGeneratorProvider:
		IEntityQueryArgumentGeneratorProvider
	{

		System.Collections.Concurrent.ConcurrentDictionary<(Type,Type), object> Creators { get; } = new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type), object>();

		Func<TSummary,TQueryArgument> GetQueryArgumentCreator<TSummary, TQueryArgument>()
		{
			var key = (typeof(TSummary), typeof(TQueryArgument));
			if (Creators.TryGetValue(key, out var f))
				return (Func<TSummary, TQueryArgument>)f;
			var p = Entity<TSummary>.GetQueryArgumentIdentProperty<TQueryArgument>();
			if (p != null)
			{
				var arg = Expression.Parameter(typeof(TSummary));
				var type = typeof(Entity<TSummary>);
				var methodGetKey = type.GetMethodExt(nameof(Entity<TSummary>.GetKey), typeof(TSummary));
				f = Expression.MemberInit(
					Expression.New(typeof(TQueryArgument)),
					Expression.Bind(
						p,
						Expression.Call(
							null,
							methodGetKey.MakeGenericMethod(p.PropertyType),
							arg
							)
						)
					).Compile<Func<TSummary, TQueryArgument>>(arg);
			}
			return (Func<TSummary, TQueryArgument>)Creators.GetOrAdd(key, f);
		}

		public IEntityQueryArgumentGenerator<TSummary, TQueryArgument> GetQueryArgumentGenerator<TSummary, TQueryArgument>() 
		{
			var func = GetQueryArgumentCreator<TSummary, TQueryArgument>();
			return new EntityQueryArgumentGenerator<TSummary, TQueryArgument>(func);
		}

		class EntityQueryArgumentGenerator<TSummary, TQueryArgument> :
			IEntityQueryArgumentGenerator<TSummary, TQueryArgument>
		{
			Func<TSummary,TQueryArgument> ArgCreator { get; }
			public EntityQueryArgumentGenerator(Func<TSummary, TQueryArgument> ArgCreator)
			{
				this.ArgCreator = ArgCreator;
			}
			public IEnumerable<QueryTestCase<TQueryArgument, TSummary>> GenerateQueryArgument(IEnumerable<TSummary> Summaries)
			{
				if (ArgCreator == null)
					return Enumerable.Empty<QueryTestCase<TQueryArgument, TSummary>>();

				return Summaries.Select(s =>
					new QueryTestCase<TQueryArgument, TSummary>
					{
						Paging = Paging.Single,
						Results = new[] { s },
						QueryArgument = ArgCreator(s)
					});
			}
		}


	}
}
