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

using SF.Auth;
using SF.Entities;
using SF.Management.Admins.Models;
using SF.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Management.Admins
{
	public class AdminQueryArgument : QueryArgument
	{
		public string Account { get; set; }
	}
	

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("系统管理员")]
	[Category("系统管理", "系统管理员管理")]
	public interface IAdminManager<TInternal,TEditable,TQueryArgument> :
		IEntitySource<long,TInternal,TQueryArgument>,
		IEntityManager<long,TEditable>
		where TInternal:AdminInternal
		where TEditable:AdminEditable
		where TQueryArgument:AdminQueryArgument
	{

	}
	public interface IAdminManager:
		IAdminManager<AdminInternal, AdminEditable, AdminQueryArgument>
	{
	}
}

