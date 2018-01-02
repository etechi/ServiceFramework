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
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Common.Comments
{
	public abstract class CommentArgumentBase
	{
		/// <summary>
		/// 标题
		/// </summary>
		[MaxLength(100)]
		public string Title { get; set; }

		/// <summary>
		/// 内容
		/// </summary>
		[MaxLength(1024)]
		public string Content { get; set; }
		
		/// <summary>
		/// Html内容
		/// </summary>
		[MaxLength(1024)]
		public string HtmlContent { get; set; }

		/// <summary>
		/// 图片列表
		/// </summary>
		public string[] Images { get; set; }
	}
	public class CommentCreateArgument : CommentArgumentBase
	{
		/// <summary>
		/// 评论对象类型
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string TargetType { get; set; }

		/// <summary>
		/// 评论对象ID
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string TargetId { get; set; }

	
	}
	public class CommentUpdateArgument : CommentArgumentBase
	{
		/// <summary>
		/// 评论ID，修改时使用
		/// </summary>
		public long? Id { get; set; }
	}
	public class CommentQueryArgument: QueryArgument
	{
		/// <summary>
		/// 评论类型
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string TargetType { get; set; }

		/// <summary>
		/// 评论对象
		/// </summary>
		[MaxLength(100)]
		[Required]
		public string TargetId { get; set; }
	}
	/// <summary>
	/// 评论服务
	/// </summary>
	/// <typeparam name="TComment">评论类型</typeparam>
	[NetworkService]
	public interface ICommentService<TComment>
		where TComment : Comment
	{
		Task<ObjectKey<long>> Create(CommentCreateArgument Arg);
		Task Update(CommentUpdateArgument Arg);
		Task Remove(long Id);
		Task<QueryResult<TComment>> Query(CommentQueryArgument Arg);
	}

	public interface ICommentService : ICommentService<Comment>
	{

	}
}
