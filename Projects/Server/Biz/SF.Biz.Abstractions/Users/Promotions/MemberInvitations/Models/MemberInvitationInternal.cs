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

using SF.Auth.Identities.Models;
using SF.Data;
using SF.Data.Models;
using SF.KB;
using SF.Metadata;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Promotions.MemberInvitations.Models
{
	[EntityObject]
	public class MemberInvitationInternal : EventEntityBase
	{
		[Comment("被邀请人ID")]
		[EntityIdent(typeof(Identity),nameof(InviteeName))]
		public override long Id { get; set; }

		[Comment("被邀请人")]
		[Hidden]
		[TableVisible]
		public string InviteeName { get; set; }

		[Comment("邀请人ID")]
		[EntityIdent(typeof(Identity), nameof(InvitorName))]
		public long InvitorId { get; set; }

		[Comment("邀请人")]
		[Hidden]
		[TableVisible]
		public string InvitorName { get; set; }

		[Hidden]
		[JsonData]
		public long[] Invitors { get; set; }

	}
}

