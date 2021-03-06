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

using SF.Sys.Annotations;
using SF.Sys.Services.Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Models
{
	public class ExtTemplateArgument
	{
		/// <summary>
		/// 参数名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 参数内容
		/// </summary>
		public string Value { get; set; }
	}
	
	public class MessageSendAction
	{
		/// <summary>
		/// 发送动作名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 是否禁用此动作
		/// </summary>
		public bool Disabled { get; set; }


		///<title>错误重发间隔</title>
		/// <summary>
		/// 错误重发间隔 (秒)。
		/// </summary>
		public int RetryInterval { get; set; }

		///<title>最大重试次数</title>
		/// <summary>
		/// 0表示不限制
		/// </summary>
		public int RetryLimit { get; set; }

		/// <summary>
		/// 消息发送服务
		/// </summary>
		[EntityIdent(typeof(ServiceInstance),ScopeValue =typeof(Senders.INotificationSendProvider))]
		public long ProviderId { get; set; }

		/// <summary>
		/// 发送目标模板
		/// </summary>
		[MaxLength(100)]
		public string TargetTemplate { get; set; }

		/// <summary>
		/// 标题模板
		/// </summary>
		[MaxLength(100)]
		public string TitleTemplate { get; set; }

		/// <summary>
		/// 内容模板
		/// </summary>
		[MultipleLines]
		public string ContentTemplate { get; set; }

		/// <summary>
		/// 外部模板ID
		/// </summary>
		[MaxLength(100)]
		public string ExtTemplateId { get; set; }

		/// <summary>
		/// 外部模板参数
		/// </summary>
		[TableRows]
		public ExtTemplateArgument[] ExtTemplateArguments { get; set; }
	}

	/// <summary>
	/// 通知策略
	/// </summary>
	[EntityObject]
	public class NotificationSendPolicy : SF.Sys.Entities.Models.ObjectEntityBase
	{
		/// <summary>
		/// 策略标识
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string Ident { get; set; }

		/// <summary>
		/// 通知标题模板
		/// </summary>
		[MaxLength(100)]
		public string NameTemplate { get; set; }

		/// <summary>
		/// 通知内容模板
		/// </summary>
		[MaxLength(1000)]
		[MultipleLines]
		public string ContentTemplate { get; set; }

		/// <summary>
		/// 发送动作
		/// </summary>
		[JsonData]
		public MessageSendAction[] Actions { get; set; }
	}
}
