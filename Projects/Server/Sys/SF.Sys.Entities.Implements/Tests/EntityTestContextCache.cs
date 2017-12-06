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
using SF.Sys.Entities.AutoTest;

namespace SF.Sys.Entities.Tests
{
	public class EntityTestHelperCache:
		IEntityTestHelperCache
	{
		IEntityCreateResultValidatorProvider[] CreateResultValidatorProviders { get; }
		IEntitySampleGeneratorProvider[] SampleGeneratorProviders { get; }
		IEntityUpdateResultValidatorProvider[] UpdateResultValidatorProviders { get; }
		IEntityDetailValidatorProvider[] DetailValidatorProviders { get; }
		IEntitySummaryValidatorProvider[] SummaryValidatorProviders { get; }
		IEntityDetailToSummaryConverterProvider[] DetailToSummaryConverterProviders { get; }
		IEntityQueryArgumentGeneratorProvider[] QueryArgumentGeneratorProviders { get; }

		public EntityTestHelperCache(
			IEnumerable<IEntityCreateResultValidatorProvider> CreateResultValidatorProviders,
			IEnumerable<IEntitySampleGeneratorProvider> EntitySampleGeneratorProviders,
			IEnumerable<IEntityUpdateResultValidatorProvider> UpdateResultValidatorProviders,
			IEnumerable<IEntityDetailValidatorProvider> DetailValidatorProviders,
			IEnumerable<IEntitySummaryValidatorProvider> SummaryValidatorProviders,
			IEnumerable<IEntityDetailToSummaryConverterProvider> DetailToSummaryConverterProviders,
			IEnumerable<IEntityQueryArgumentGeneratorProvider> QueryArgumentGeneratorProviders
			)
		{
			this.CreateResultValidatorProviders = CreateResultValidatorProviders.ToArray();
			this.SampleGeneratorProviders = EntitySampleGeneratorProviders.ToArray();
			this.UpdateResultValidatorProviders = UpdateResultValidatorProviders.ToArray();
			this.DetailToSummaryConverterProviders = DetailToSummaryConverterProviders.ToArray();
			this.SummaryValidatorProviders = SummaryValidatorProviders.ToArray();
			this.DetailValidatorProviders = DetailValidatorProviders.ToArray();
			this.QueryArgumentGeneratorProviders = QueryArgumentGeneratorProviders.ToArray();
		}

		System.Collections.Concurrent.ConcurrentDictionary<(Type, Type, Type, Type), object> Testors { get; }
			= new System.Collections.Concurrent.ConcurrentDictionary<(Type, Type, Type, Type), object>();
		public IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> GetTestHelper<TDetail, TSummary, TEditable, TQueryArgument>() where TQueryArgument:IPagingArgument
		{
			var key = (typeof(TDetail), typeof(TSummary), typeof(TEditable), typeof(TQueryArgument));
			if (Testors.TryGetValue(key, out var testor))
				return (IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument>)testor;
			return (IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument>)
				Testors.GetOrAdd(key,
				new EntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument>(
					DetailToSummaryConverterProviders.Select(p => p.GetDetailToSummaryConverter<TDetail, TSummary>()).First(c => c != null),
					SampleGeneratorProviders.Select(p => p.GetSampleGenerator<TEditable>()).Where(p=>p!=null).OrderBy(t => t.Priority).ToArray(),
					QueryArgumentGeneratorProviders.Select(p => p.GetQueryArgumentGenerator<TSummary, TQueryArgument>()).Where(p => p != null).ToArray(),
					CreateResultValidatorProviders.Select(p => p.GetCreateResultValidator<TEditable>()).Where(p => p != null).ToArray(),
					DetailValidatorProviders.Select(p => p.GetDetailValidator<TEditable, TDetail>()).Where(p => p != null).ToArray(),
					SummaryValidatorProviders.Select(p => p.GetSummaryValidator<TDetail, TSummary>()).Where(p => p != null).ToArray(),
					UpdateResultValidatorProviders.Select(p => p.GetUpdateResultValidator<TEditable>()).Where(p => p != null).ToArray()
					)
				);
		}
	}
}
