﻿#region Apache License Version 2.0
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
	public static class ClientManagerExtension
	{
		public static async Task<ClientInternal> ClientEnsure(
			this IClientManager ClientManager,
			string Id,
			string Name,
			long ConfigId,
			string Secret,
			string Title=null,
			string[] CredentialTypeIdents=null
			) 
		{
			return await ClientManager.EnsureEntity(
				ObjectKey.From(Id),
				() => new ClientEditable
				{
					Id = Id
				},
				e =>
				{
					e.Name = Name;
					e.Name = Title??Name;
					e.ClientSecrets = Secret;
					e.ClientConfigId = ConfigId;
					e.RedirectUris = "http://localhost:52706/";
				}
				);
		}
	}

}

