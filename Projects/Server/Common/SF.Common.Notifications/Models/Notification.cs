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
using SF.Sys.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Models
{
	[EntityObject]
	public class Notification : IEntityWithId<long>
	{
		/// <summary>
		/// ID
		/// </summary>
		[Key]
		[ReadOnly(true)]
		[TableVisible]
		[Layout(10)]
		public long Id { get; set; }

		/// <summary>
		/// 通知模式
		/// </summary>
		[Uneditable]
		public NotificationMode Mode { get; set; }

		///<title>通知时间</title>
		/// <summary>
		/// 只有当前时间到达开始时间时，用户才能看到通知,默认为当前时间
		/// </summary>
		[TableVisible]
		[Uneditable]
		public DateTime Time { get; set; }


		/// <summary>
		/// 通知发送策略
		/// </summary>
		[EntityIdent(typeof(NotificationSendPolicy),nameof(PolicyName))]
		public long? PolicyId { get; set; }

		/// <summary>
		/// 通知发送策略
		/// </summary>
		[TableVisible]
		[Ignore]
		public string PolicyName { get; set; }

		///<title>过期时间</title>
		/// <summary>
		/// 通知过期时间，超过该时间后，不会再在用户的通知列表中出现。
		/// </summary>
		[TableVisible]
		[Uneditable]
		public DateTime Expires { get; set; }

		///<title>主要通知对象</title>
		/// <summary>
		/// 一般通知有效
		/// </summary>
		[EntityIdent(typeof(User), nameof(TargetName))]
		[Uneditable]
		public long? TargetId { get; set; }

		/// <summary>
		/// 主要通知对象
		/// </summary>
		[TableVisible]
		[Ignore]
		public string TargetName { get; set; }


		///<title>通知标题</title>
		/// <summary>
		/// 通知的标题，为方便用户在移动设备查看，尽可能在标题中说明事情。
		/// </summary>
		[MaxLength(100)]
		[Required]
		[TableVisible]
		[EntityTitle]
		[Layout(30)]
		public string Name { get; set; }

		///<title>通知链接</title>
		/// <summary>
		/// 用户可以通过该链接跳转至指定页面
		/// </summary>
		[MaxLength(100)]
		[Layout(40)]
		public string Link { get; set; }

		///<title>通知图片</title>
		/// <summary>
		/// 会显示在通知标题前，一般通知不需要图片
		/// </summary>
		[Image]
		[Layout(50)]
		public string Image { get; set; }

		///<title>通知详细内容</title>
		/// <summary>
		/// 需要点击标题进行查看，一般只有促销类的通知才需要通知详细内容
		/// </summary>
		[Html]
		[MaxLength(2000)]
		[Layout(60)]
		public string Content { get; set; }

		///<title>发布人</title>
		/// <summary>
		/// 发布用户,可以为空
		/// </summary>
		[EntityIdent(typeof(User), nameof(SenderName))]
		[Layout(70)]
		[Uneditable]
		public long? SenderId { get; set; }

		/// <summary>
		/// 发布人
		/// </summary>
		[Ignore]
		[TableVisible]
		public string SenderName { get; set; }

		/// <summary>
		/// 业务跟踪对象
		/// </summary>
		[EntityIdent]
		[Layout(80)]
		public string BizIdent { get; set; }

		/// <summary>
		/// 投递次数
		/// </summary>
		[ReadOnly(true)]
		public int SendCount { get; set; }

		/// <summary>
		/// 投递完成次数
		/// </summary>
		[ReadOnly(true)]
		public int CompletedSendCount { get; set; }


	}

	public class NotificationEditable : Notification
	{
		///<title>通知对象</title>
		/// <summary>
		/// 普通通知的发送对象，全体通知留空
		/// </summary>
		[EntityIdent(typeof(User))]
		[Layout(26)]
		[Required]
		[Range(1, 100)]
		[Uneditable]
		public IEnumerable<long> Targets { get; set; }

		/// <summary>
		/// 通知记录
		/// </summary>
		[TableRows]
		public IEnumerable<NotificationSendRecord> SendRecords { get; set; }


		/// <summary>
		/// 消息参数
		/// </summary>
		[JsonData]
		[Uneditable]
		public Dictionary<string, object> Args { get; set; }

	}
}
