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

namespace SF.Entities.Tests
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
		public IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> GetTestHelper<TDetail, TSummary, TEditable, TQueryArgument>()
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
