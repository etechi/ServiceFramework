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

using SF.Sys.Linq;
using SF.Sys.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace SF.Sys.Comments
{
	public static class MemberComment
	{
		static ConcurrentDictionary<MemberInfo, Comment> CommentDict { get; } 
			= new ConcurrentDictionary<MemberInfo, Comment>();
		public static Comment Comment(this MemberInfo Member)
		{
			Comment comment;
			if (CommentDict.TryGetValue(Member, out comment))
				return comment;
			var id = Member.GetMemberXmlDocId();
			var type = Member as Type;
			var c = (type == null ?
					XmlCodeDocument.GetComment(type) :
					type.AllInterfaces()
						.WithFirst(type)
						.Select(i => XmlCodeDocument.GetComment(i))
						.FirstOrDefault(i => i != null)
					)
					?? new Comment(id, Member.ShortName());
			if (type != null && type.IsGeneric())
			{
				var gtd = type.GetGenericTypeDefinition();
				var gta = type.GenericTypeArguments;
				var baseName = c.Title.TrimEndTo("<", true, true);
				c = new Comment(
					null,
					$"{gta.Select(a => a.Comment().Title).Join(" ")} {baseName}",
					$"{gta.Select(a => a.Comment().Summary).Join(" ")} {c.Summary}",
					$"{gta.Select(a => a.Comment().Group).Join(" ")} {c.Group}",
					$"{gta.Select(a => a.Comment().Remarks).Join(" ")} {c.Remarks}",
					$"{gta.Select(a => a.Comment().Prompt).Join(" ")} {c.Prompt}"
					);
			}

			return CommentDict.GetOrAdd(
				Member,
				c
				);
		}
		
		public static string FriendlyName(this MemberInfo Info)
		{
			return Info.Comment()?.Title ?? Info.ShortName();
		}

	}
}