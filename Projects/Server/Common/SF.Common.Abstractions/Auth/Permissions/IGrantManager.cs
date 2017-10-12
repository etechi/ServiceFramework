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
using SF.Entities;
using SF.Metadata;
using SF.Auth.Identities;
using SF.Auth.Identities.Models;

namespace SF.Auth.Permissions
{
	public class GrantQueryArgument : ObjectQueryArgument<long>
	{
		[EntityIdent(typeof(Identity))]
		public long? IdentityId { get; set; }
	}
	public interface IGrantManager<TGrantEditable>
        : IEntitySource<ObjectKey<long>, TGrantEditable, GrantQueryArgument>,
		IEntityManager<ObjectKey<long>, TGrantEditable>
		where TGrantEditable : Models.GrantEditable
	{
	}
	public interface IGrantManager : IGrantManager<Models.GrantEditable>
	{ }
}
