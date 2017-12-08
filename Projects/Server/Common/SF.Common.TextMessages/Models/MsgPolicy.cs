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
using SF.Sys.Services.Management.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Common.TextMessages.Models
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
		/// 发送多种名称
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 是否禁用此动作
		/// </summary>
		public bool Disabled { get; set; }

		/// <summary>
		/// 错误重发次数
		/// </summary>
		public int RetryCount { get; set; }

		///<title>错误重发间隔</title>
		/// <summary>
		/// 错误重发间隔 (毫秒),注意客户端发送消息重试时间过长会产生用户体验问题。
		/// </summary>
		[Range(0,60*1000)]
		public int RetryInterval { get; set; }

		/// <summary>
		/// 消息发送服务
		/// </summary>
		[EntityIdent(typeof(ServiceInstance),ScopeValue =typeof(IMsgProvider))]
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

	[EntityObject]
	public class MsgPolicy : SF.Sys.Entities.Models.ObjectEntityBase
	{
		/// <summary>
		/// 策略标识
		/// </summary>
		[Required]
		[MaxLength(100)]
		public string Ident { get; set; }

		/// <summary>
		/// 发送动作
		/// </summary>
		public MessageSendAction[] Actions { get; set; }
	}
}
