
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
	class EntityTestContext<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager> :
		IEntityTestContext<TKey, TDetail, TSummary, TEditable, TQueryArgument, TManager>
		where TManager :
			IEntitySource<TKey, TSummary, TDetail, TQueryArgument>,
			IEntityManager<TKey, TEditable>
	{
		public EntityTestContext(
			TManager Manager, 
			ITestAssert TestAssert, 
			IEntityTestHelperCache EntityTestHelperCache
			)
		{
			this.Manager = Manager;
			this.Assert = TestAssert;
			this.Helper = EntityTestHelperCache.GetTestHelper<TDetail,TSummary,TEditable,TQueryArgument>();
		}
		public TManager Manager { get; }


		public ITestAssert Assert { get; }

		public IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> Helper { get; }
	}
}
