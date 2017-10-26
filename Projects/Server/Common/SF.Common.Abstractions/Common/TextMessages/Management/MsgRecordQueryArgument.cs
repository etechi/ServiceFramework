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

using SF.Auth.Users.Models;
using SF.Core.ServiceManagement.Models;
using SF.Entities;
using SF.Metadata;
using System;

namespace SF.Common.TextMessages.Management
{

	public class MsgRecordQueryArgument : Entities.QueryArgument<ObjectKey<long>>
	{
		[Comment( "状态")]
		public SendStatus? Status { get; set; }

		[Comment( "目标用户")]
		[EntityIdent(typeof(User))]
		public long? TargeUserId { get; set; }


		[Comment( "发送服务")]
		[EntityIdent(typeof(ServiceInstance))]
		public long? ServiceId { get; set; }

		[Comment( "发送时间")]
		public QueryRange<DateTime> Time { get; set; }

		[Comment( "发送对象")]
		public string Target { get; set; }
	}

}
