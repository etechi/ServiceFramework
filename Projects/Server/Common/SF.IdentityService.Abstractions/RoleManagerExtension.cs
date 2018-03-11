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

using System.Threading.Tasks;
using SF.Auth.IdentityServices.Models;
using SF.Sys.Entities;

namespace SF.Auth.IdentityServices.Managers
{
	public static class RoleManagerExtension
	{
		public static async Task<Role> RoleEnsure(
			this IRoleManager RoleManager,
			string Id,
			string Name,
			bool IsSysRole
		//	params Grant[] Grants
			) 
		{
			return await RoleManager.EnsureEntity(
				await RoleManager.QuerySingleEntityIdent(new RoleQueryArgument { Id = new ObjectKey<string> { Id = Id } }),
				() => new RoleEditable
				{
					Id = Id
				},
				e =>
				{
					e.Name = Name;
			//		e.Grants = Grants;
					e.IsSysRole = IsSysRole;
				}
			);
		}
	}

}

