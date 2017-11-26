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

		static string GetTypeName(Type Type)
		{
			if (Type.IsPointer) return GetTypeName(Type.GetElementType()) + "*";
			else if (Type.IsArray) return GetTypeName(Type.GetElementType()) + "[]";
			else return Type.FullName;
		}
		static string GetMethodArguments(MethodBase Method)
		{
			var ps = Method.GetParameters();
			if (ps.Length == 0) return "";
			return "(" + ps.Select(p => GetTypeName(p.ParameterType)).Join(",") + ")";
		}
		public static string GetMemberXmlDocId(this MemberInfo Member)
		{
			if (Member is PropertyInfo prop)
			{
				return "P:" + prop.DeclaringType.FullName + "." + prop.Name;
			}
			else if (Member is FieldInfo field)
			{
				return "F:" + field.DeclaringType.FullName + "." + field.Name;
			}
			else if (Member is MethodInfo method)
			{
				return "M:" + method.DeclaringType.FullName + "." + method.Name+GetMethodArguments(method);
			}
			else if(Member is ConstructorInfo ctr)
			{
				return "M:" + ctr.DeclaringType.FullName + ".#ctor" + GetMethodArguments(ctr);

			}
			else if (Member is Type type)
			{
				return "T:" + GetTypeName(type);
			}
			else
				return "";
		}
		static Comment ReadXmlComment(XElement e)
		{
			var id = e.Attribute("name")?.Value;
			var summary = e.Element("summary")?.Value?.Trim();
			var title = e.Element("title")?.Value?.Trim();
			var remarks = e.Element("remarks")?.Value?.Trim();
			var group = e.Element("group")?.Value?.Trim();
			var prompts = e.Element("prompt")?.Value?.Trim();
			var order = e.Element("order")?.Value?.Trim();
			return new Comment(
				id,
				title ?? summary,
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
			var docFile = Assembly.CodeBase
				.LastSplit2('.')
				.Item1
				.TrimStart("file:///")
				.Replace('/',System.IO.Path.AltDirectorySeparatorChar) 
				+ ".xml";

			dict = System.IO.File.Exists(docFile) ?
				XDocument.Load(docFile)
					.Root
					//.Element("doc")
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
			if (Member == null) throw new ArgumentException("必须提供成员");
			var type=(Member.MemberType & MemberTypes.TypeInfo )== MemberTypes.TypeInfo?(Type)Member:Member.DeclaringType;
			var assComments = type.Assembly.GetAssemblyComments();
			var id = Member.GetMemberXmlDocId();
			if (assComments.TryGetValue(id, out var comment))
				return comment;
			return new Comment(id, Member.Name);
		}
	}
		

}