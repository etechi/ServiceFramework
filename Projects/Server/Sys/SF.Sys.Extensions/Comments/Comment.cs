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

using System;
using System.Collections.Generic;

namespace SF.Sys.Comments
{
	public class Comment
	{

		public Comment(
			string Id,
			string Title,
			string Summary = null,
			string Group = null,
			string Remarks = null,
			string Prompts = null,
			int Order = 0,
			bool IsDefaultComment=false,
			IReadOnlyDictionary<string, Comment> Parameters=null,
			string Returns=null
			)
		{
			this.IsDefaultComment = IsDefaultComment;
			this.Id = Id;
			this.Title = Title;
			this.Summary = Summary;
			this.Group = Group;
			this.Remarks = Remarks;
			this.Order = Order;
			this.Prompt = Prompts;
			this.Parameters = Parameters;
			this.Returns = Returns;
		}
		public override string ToString()
		{
			return Title + ":" + Summary;
		}
		public bool IsDefaultComment { get; }
		public string Id { get; }
		public string Summary { get; }
		public string Prompt { get; }
		public string Group { get; }
		public string Title { get; }
		public int Order { get; }
		public string Remarks { get; }
		public string ShortName { get; }
		public IReadOnlyDictionary<string,Comment> Parameters { get; }
		public string Returns { get; }
	}

}
