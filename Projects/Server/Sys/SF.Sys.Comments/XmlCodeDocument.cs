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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace SF.Sys.Comments
{
	
	public static class XmlCodeDocument
	{
		static ConcurrentDictionary<string, IReadOnlyDictionary<string, Comment>> CommentDict { get; }
			= new ConcurrentDictionary<string, IReadOnlyDictionary<string, Comments.Comment>>();

		public static string GetMemberXmlDocId(this MemberInfo Member)
		{
			return null;
		}
		static Comment ReadXmlComment(XElement e)
		{
			var id = e.Attribute("name")?.Value;
			var summary = e.Element("summary")?.Value;
			var title = e.Element("title")?.Value;
			var remarks = e.Element("remarks")?.Value;
			var group = e.Element("group")?.Value;
			var prompts = e.Element("prompts")?.Value;
			var order = e.Element("order")?.Value;
			return new Comment(
				id,
				title,
				summary,
				group,
				remarks,
				prompts,
				order.TryToInt32() ?? 0
				);
		}
		public static IReadOnlyDictionary<string, Comment> GetAssemblyComments(this Assembly Assembly)
		{
			if (CommentDict.TryGetValue(Assembly.FullName, out var dict))
				return dict;
			var docFile = Assembly.CodeBase.LastSplit2('.').Item1 + ".xml";
			dict = System.IO.File.Exists(docFile) ?
				XDocument.Load(docFile)
					.Root
					.Element("doc")
					.Element("members")
					.Elements()
					.Select(e => ReadXmlComment(e))
					.Where(e => e.Id.HasContent())
					.ToDictionary(e => e.Id)
				: new Dictionary<string, Comment>();
			return CommentDict.GetOrAdd(Assembly.FullName, dict);
		}
		public static Comment GetComment(this MemberInfo Member)
		{
			var assComments = Member.DeclaringType.Assembly.GetAssemblyComments();
			var id = Member.GetMemberXmlDocId();
			if (assComments.TryGetValue(id, out var comment))
				return comment;
			return new Comment(id, Member.Name);
		}
	}
		

}
