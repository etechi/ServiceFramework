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

using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities.Models;

namespace SF.Biz.MemberInvitations.Models
{
	[EntityObject]
	public class MemberInvitationInternal : EventEntityBase
	{
		/// <summary>
		/// 被邀请人ID
		/// </summary>
		[EntityIdent(typeof(User),nameof(InviteeName))]
		public override long Id { get; set; }

		/// <summary>
		/// 被邀请人
		/// </summary>
		[Hidden]
		[TableVisible]
		public string InviteeName { get; set; }

		/// <summary>
		/// 邀请人ID
		/// </summary>
		[EntityIdent(typeof(User), nameof(InvitorName))]
		public long InvitorId { get; set; }

		/// <summary>
		/// 邀请人
		/// </summary>
		[Hidden]
		[TableVisible]
		public string InvitorName { get; set; }

		[Hidden]
		[JsonData]
		public long[] Invitors { get; set; }

	}
}

