using System;
using System.Collections.Generic;
using System.Linq;
using SF.Services.Tests;

namespace SF.Entities.Tests
{
	
	public interface IEntityTestContext<TKey,TDetail, TSummary, TEditable, TQueryArgument, TManager>:
		ITestContext
		where TManager:
			IEntitySource<TKey,TSummary,TDetail,TQueryArgument>,
			IEntityManager<TKey,TEditable>
	{
		TManager Manager { get; }
		IEntityTestHelper<TDetail,TSummary,TEditable,TQueryArgument> Helper { get; }
	}

	public interface IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> :
		IEntityCreateResultValidator<TEditable>,
		IEntitySampleGenerator<TEditable>,
		IEntityUpdateResultValidator<TEditable>,
		IEntityDetailValidator<TEditable, TDetail>,
		IEntitySummaryValidator<TDetail, TSummary>,
		IEntityQueryArgumentGenerator<TSummary, TQueryArgument>,
		IEntityDetailToSummaryConverter<TDetail, TSummary>
	{
	}

	public interface IEntityTestHelperCache
	{
		IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> GetTestHelper<TDetail, TSummary, TEditable, TQueryArgument>();
	}


}
