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
using SF.Metadata;
using System;
namespace SF.Auth.IdentityServices.Managers
{
	public class ClientQueryArgument : IQueryArgument<ObjectKey<string>>
	{
		public ObjectKey<string> Id { get; set; }

		[Comment("客户端名称")]
		[StringContains]
		public string Name { get; set; }
	}

	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("客户端管理")]
	public interface IClientManager :
		IEntityManager<ObjectKey<string>,Models.ClientEditable>,
		IEntitySource<ObjectKey<string>, Models.ClientInternal, ClientQueryArgument>
	{
	}

}

