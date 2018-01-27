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
using System.Linq;
using System.Threading.Tasks;
using SF.Common.Comments.DataModels;
using SF.Sys.Data;
using SF.Sys.Services;
using SF.Sys.Entities;

namespace SF.Common.Comments
{
	public class CommentService : 
		CommentService<Comment, DataModels.Comment>,
		ICommentService
	{
		public CommentService(IDataScope DataScope) : 
			base(DataScope)
		{
		}
	}

	public class CommentService<TCommentPublic, TComment> :
		ICommentService<TCommentPublic>
		where TCommentPublic : Comment, new()
		where TComment : DataModels.Comment<TComment>, new()

	{
		public IDataScope DataScope { get; }
		public CommentService(IDataScope DataScope)
		{
			this.DataScope = DataScope;
		}

		public Task<TCommentPublic> GetAsync(ObjectKey<long> Id)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<TCommentPublic>> ListChildContainersAsync(ListItemsArgument<long?> Arg)
		{
			throw new NotImplementedException();
		}

		public Task<ObjectKey<long>> Create(CommentCreateArgument Arg)
		{
			throw new NotImplementedException();
		}

		public Task Update(CommentUpdateArgument Arg)
		{
			throw new NotImplementedException();
		}

		public Task Remove(long Id)
		{
			throw new NotImplementedException();
		}

		public Task<QueryResult<TCommentPublic>> Query(CommentQueryArgument Arg)
		{
			throw new NotImplementedException();
		}
	}
}
