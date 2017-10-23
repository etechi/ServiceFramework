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

using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;
using SF.Core.ServiceManagement;
using SF.Core;
using SF.Auth.Users;
using SF.Management.BizAdmins.Models;

namespace SF.Management.BizAdmins
{
	public static class BizAdminExtension
	{
		public static async Task<BizAdminInternal> BizAdminEnsure(
			this IBizAdminService Service,
			IServiceProvider ServiceProvider,
			string name,
			string nick,
			string account,
			string password,
			string[] roles = null,
			Dictionary<string, string> extArgs = null
			) 
		{
			return await Service.UserEnsure<IBizAdminManagementService, BizAdminInternal, BizAdminEditable, BizAdminQueryArgument>(
				ServiceProvider,
				name, nick,
				account,
				password,
				roles,
				extArgs
				);

		}
	}

}

