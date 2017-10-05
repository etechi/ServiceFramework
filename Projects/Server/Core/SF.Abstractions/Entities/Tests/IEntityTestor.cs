using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Entities.Tests
{
	public interface IEntityTestDataProvider<TEditable>
	{
		Task<T> UseTestEntity<T>(Func<TEditable[], Task<T>> Action);
	}
	public interface IEntityTestor<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>
		where TKey : new()
		where TDetail : new()
		where TEditable : new()
		where TManager : 
			IEntitySource<TKey,TSummary,TDetail,TQueryArgument>,
			IEntityManager<TKey,TEditable>
	{
		Task Test(
			TManager svc,
			IEntityTestContext<TDetail, TSummary, TEditable, TQueryArgument> Context,
			int CreateCount
			);
	}

}
