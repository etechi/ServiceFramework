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
using SF.Sys.Entities;
using SF.Sys.CallPlans.Models;

namespace SF.Sys.CallPlans
{

	public class CallInstanceManager :
		ModidifiableEntityManager<ObjectKey<string, string>, CallInstance, CallInstanceQueryArgument, CallInstanceEditable, DataModels.CallInstance>,
		ICallInstanceManager
	{
		public CallInstanceManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}

		protected override PagingQueryBuilder<DataModels.CallInstance> PagingQueryBuilder => throw new NotImplementedException();

		protected override IContextQueryable<DataModels.CallInstance> OnBuildQuery(IContextQueryable<DataModels.CallInstance> Query, CallInstanceQueryArgument Arg)
		{
			throw new NotImplementedException();
		}

		protected override Task OnUpdateModel(IModifyContext ctx)
		{
			throw new NotImplementedException();
		}
	}
}
