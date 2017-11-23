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



using SF.Common.Admins.Models;
using SF.Sys.Entities;

namespace SF.Common.Admins
{
	public class AdminManager
		: AdminManager<AdminInternal, AdminEditable, AdminQueryArgument, DataModels.Admin>,
		IAdminManager
	{
		public AdminManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}
	public class AdminManager<TInternal, TEditable, TQueryArgument, TAdmin> :
		AutoModifiableEntityManager<long, TInternal, TInternal, TQueryArgument, TEditable, TAdmin>,
		IAdminManager<TInternal, TEditable, TQueryArgument>
		where TInternal : AdminInternal
		where TEditable : AdminEditable
		where TQueryArgument : AdminQueryArgument,new()
		where TAdmin : DataModels.Admin<TAdmin>, new()
	{
		public AdminManager(IEntityServiceContext ServiceContext) : base(ServiceContext)
		{
		}
	}

}
