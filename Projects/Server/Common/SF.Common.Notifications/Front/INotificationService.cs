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

using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Front
{

	public class SetReadedArgument
	{
		/// <summary>
		/// 需要设置为已读状态的通知ID
		/// </summary>
		public long[] NotificationIds { get; set; }
	}
	public class NotificationQueryArgument : QueryArgument {
		/// <summary>
		/// 是否查询
		/// </summary>
		public NotificationMode? Mode { get; set; }
		public bool? Readed { get; set; }
	}

	/// <summary>
	/// 用户通知服务
	/// </summary>
	[NetworkService]
	public interface INotificationService
	{
		/// <summary>
		/// 获取普通通知，带标题
		/// </summary>
		/// <param name="Id">通知ID</param>
		/// <returns></returns>
		Task<UserNotification> Get(long Id);

		/// <summary>
		/// 查询通知
		/// </summary>
		/// <param name="Arg">查询参数</param>
		/// <returns></returns>
		Task<QueryResult<UserNotification>> Query(NotificationQueryArgument Arg);

		/// <summary>
		/// 删除通知
		/// </summary>
		/// <param name="NotificationId"></param>
		/// <returns></returns>
		Task Delete(long NotificationId);

		/// <summary>
		/// 获取通知未读状态
		/// </summary>
		/// <returns></returns>
		Task<UserNotificationStatus> GetStatus();

		/// <summary>
		///将指定通知设置为已读状态
		/// </summary>
		/// <param name="Arg"></param>
		/// <returns></returns>
		Task SetReaded(SetReadedArgument Arg);

	}

}
