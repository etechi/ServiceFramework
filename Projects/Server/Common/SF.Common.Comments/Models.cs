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


using SF.Sys.Entities.Models;
using SF.Sys.Annotations;
using System.Collections.Generic;
using SF.Sys.Entities;
using System.ComponentModel.DataAnnotations;
using SF.Sys.Auth;

namespace SF.Common.Comments
{
	/// <summary>
	/// 文档实体
	/// </summary>
	[EntityObject]
	public class Comment : IEntityWithId<long>
	{
		[Key]
		public long Id { get; }

		/// <summary>
		/// 评论人
		/// </summary>
		[EntityIdent(typeof(User),nameof(UserName))]
		public long UserId { get; set; }

		/// <summary>
		/// 评论人昵称
		/// </summary>
		public string UserName { get; set; }


		/// <summary>
		/// 评论人图片
		/// </summary>
		[FromEntityProperty(nameof(UserId),nameof(User.Icon))]
		public string UserIcon { get; set; }

		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreatedTime { get; set; }

		/// <summary>
		/// 更新时间
		/// </summary>
		public string UpdatedTime { get; set; }

		/// <summary>
		/// 标题
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// 引用
		/// </summary>
		public Comment Refer{ get; set; }

		/// <summary>
		/// 图片列表
		/// </summary>
		public string[] ImageList { get; set; }

		/// <summary>
		/// 文档内容
		/// </summary>
		[MultipleLines]
		public string Content { get; set; }

		/// <summary>
		/// 文档内容,Html格式
		/// </summary>
		[Html]
		public string HtmlContent { get; set; }


	}
	
}
