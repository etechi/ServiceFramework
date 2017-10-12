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
using SF.Auth.Identities;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Metadata;
using SF.Users.Promotions.MemberSources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Users.Promotions.MemberSources
{
	public class MemberSourceQueryArgument : Entities.IQueryArgument<ObjectKey<long>>
	{
		[Comment("Id")]
		public ObjectKey<long> Id { get; set; }

		[Comment("名称")]
		public string Name { get; set; }
	}


	[EntityManager]
	[Authorize("admin")]
	[NetworkService]
	[Comment("会员渠道")]
	[Category("用户管理", "会员渠道管理")]
	public interface IMemberSourceManagementService : 
		Entities.IEntitySource<ObjectKey<long>,MemberSourceInternal,MemberSourceQueryArgument>,
		Entities.IEntityManager<ObjectKey<long>, MemberSourceInternal>
    {
		Task AddSourceMember(long SourceId, long MemberId);
	}

}

