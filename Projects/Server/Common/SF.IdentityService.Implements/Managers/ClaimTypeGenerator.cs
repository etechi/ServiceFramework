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

using SF.Auth.Users.Internals;
using SF.Auth.Users.Models;
using SF.Core;
using SF.Core.ServiceManagement;
using SF.Core.Times;
using SF.Data;
using SF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices.Managers
{
	public static class ClaimTypeProvider
	{
		public static async Task<long> GetOrCreateClaimType(
			this IEntityManager EntityManager,
			string Type
			)
		{
			var scope = await EntityManager.DataContext.Set<DataModels.ClaimType>().GetOrCreateAtomEntity(
				EntityManager.ScopedDataContext,
				n => n.Name == Type,
				async () => new DataModels.ClaimType
				{
					Id = await EntityManager.IdentGenerator.GenerateAsync(typeof(DataModels.ClaimType).FullName),
					Name = Type
				}
				);
			return scope.Id;
		}
	}

}
