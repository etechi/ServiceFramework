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
			IEntityTestHelperCache EntityTestHelperCache,
			ISampleSeed SampleSeed
			)
		{
			this.SampleSeed = SampleSeed;
			this.Manager = Manager;
			this.Assert = TestAssert;
			this.Helper = EntityTestHelperCache.GetTestHelper<TDetail,TSummary,TEditable,TQueryArgument>();
		}
		public TManager Manager { get; }


		public ITestAssert Assert { get; }

		public IEntityTestHelper<TDetail, TSummary, TEditable, TQueryArgument> Helper { get; }

		public ISampleSeed SampleSeed { get; }
	}
}
