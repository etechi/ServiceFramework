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

using System;
using System.Collections.Generic;
using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Sys.Data;
using System.ComponentModel.DataAnnotations;

namespace SF.Common.Comments.DataModels
{
	public class Comment : Comment<Comment>
	{ }

	/// <summary>
	/// 文档
	/// </summary>
	/// <typeparam name="TComment"></typeparam>
	[Table("Comment")]
	public class Comment<TComment> :
		DataObjectEntityBase
		where TComment : Comment<TComment>
	{
		/// <summary>
		/// 评论类型
		/// </summary>
		[Index]
		public string TargetType { get; set; }
		
		/// <summary>
		/// 对象
		/// </summary>
		[Index]
		[MaxLength(100)]
        public string TargetId { get; set; }

		/// <summary>
		/// 评论内容
		/// </summary>
		[MaxLength(1000)]
		public string Content { get; set; }

		/// <summary>
		/// HTML内容
		/// </summary>
		[MaxLength(1000)]
		public string HtmlContent { get; set; }

		/// <summary>
		/// 图片列表
		/// </summary>
		[MaxLength(1000)]
		public string Images { get; set; }

	}
}
