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

using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SF.Sys.NetworkService
{

	public class JavaProxyBuilder
    {
        Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }
		Dictionary<string, SF.Sys.Metadata.Models.Type> Types;
		System.IO.Compression.ZipArchive Archive;
		public string PackagePath{ get; set; }
		public string CommonImports { get; set; }
        public JavaProxyBuilder(
			string CommonImports,
			string PackagePath,
			Func<Metadata.Service, Metadata.Method, bool> ActionFilter
			)
        {
			this.CommonImports = CommonImports;
			this.PackagePath = PackagePath;
			this.ActionFilter = ActionFilter;
        }
		bool IsEnumType(string type)
		{
			if (Types.TryGetValue(type, out var t))
				return t.IsEnumType;
			return false;
		}
		HashSet<string> Imported { get; } = new HashSet<string>();
		bool TryImport(StringBuilder sb,string type)
		{
			if (Imported.Add(type) && Types.TryGetValue(type, out var st))
				BuildType(st);

			if (type.EndsWith("?"))
				return TryImport(sb,type.Substring(0, type.Length - 1));

			var i = type.IndexOf('[');
			if (i != -1)
				return TryImport(sb,type.Substring(0, i));

			i = type.IndexOf('{');
			if (i != -1)
				return TryImport(sb,type.Substring(0, i));

			if (IsEnumType(type))
				return false;
			i = type.IndexOf('<');
			if (i != -1)
				type=type.Replace('.', '_').Replace('<', '_').Replace(',', '_').Replace('>', '_');

			switch (type)
			{
				case "string": 
				case "object": 
				case "datetime": 
				case "timespan":
				case "long":
				case "int":
				case "short":
				case "sbyte":

				case "ulong":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
				case "decimal":
				case "float":
				case "double":
				case "bool":
				case "void":
				case "unknown":
					return false;
					
			}
			sb.AppendLine($"import {PackagePath}.{type.Replace('.', '_').Replace('+', '_')};");
			return true;
		}
		string to_java_type(string type,bool ObjectMode=false,bool EscapeEnumName=true)
		{
			if (type.EndsWith("?"))
				return to_java_type(type.Substring(0, type.Length - 1), ObjectMode);

			var i = type.IndexOf('[');
			if (i != -1)
				return to_java_type(type.Substring(0, i), ObjectMode) + type.Substring(i);

			i = type.IndexOf('{');
			if (i != -1)
				return "HashMap<String," + to_java_type(type.Substring(0, i), true) + ">";

			if (EscapeEnumName && IsEnumType(type))
				return "String";

			i = type.IndexOf('<');
			if (i != -1)
				return type.Replace('.', '_').Replace('<', '_').Replace(',', '_').Replace('>', '_');

			
			switch (type)
			{
				case "string": return "String";
				case "object":return "Object";
				case "datetime": return "String";
                case "timespan":return "String";
                case "long": 
				case "ulong": return ObjectMode ? "Long" : "long";
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
					return ObjectMode ? "Integer" : "int";
				case "decimal":
				case "float":
				case "double":
					return ObjectMode ? "Double" : "double";
				case "bool":
					return ObjectMode ? "Boolean" : "double";
				case "void":
				case "unknown":
					return "Object";
				default:
					return type.Replace('.', '_').Replace('+','_');
			}
		}
		void AddFile(string name,Action<StringBuilder> content)
		{
			using (var s = Archive.CreateEntry(name + ".java").Open())
			{
				var sb = new StringBuilder();
				sb.AppendLine($"package {PackagePath};");
				sb.AppendLine(CommonImports);
				content(sb);
				var buf = Encoding.UTF8.GetBytes(sb.ToString());
				s.Write(buf, 0, buf.Length);
			}
		}
		void BuildEnumType(SF.Sys.Metadata.Models.Type t)
		{
			AddFile(to_java_type(t.Name,false,false), (sb) =>
			{
				sb.AppendLine($"/**");
				sb.AppendLine($"* {t.Title}");
				sb.AppendLine($"* {t.Description}");
				sb.AppendLine($"*/");
				sb.AppendLine($"public class {to_java_type(t.Name,false,false)} {{");
				foreach (var p in t.Properties)
				{
					sb.AppendLine($"/**");
					sb.AppendLine($"* {t.Title}");
					sb.AppendLine($"* {t.Description}");
					sb.AppendLine($"*/");
					sb.AppendLine($"\tpublic static final String {p.Name}=\"{p.Name}\";");
				}
				sb.AppendLine("}");
			});
		}
		void BuildDictType(SF.Sys.Metadata.Models.Type t)
		{
			//sb.AppendLine($"// {t.Title}");
			//sb.AppendLine($"export interface {to_js_type(t.Name)} = {{[index:string]:{to_js_type(t.ElementType)}}};");
		}
		bool IsOverrideProperty(string Name, SF.Sys.Metadata.Models.Type type)
		{
			if (type.BaseTypes == null)
				return false;
			foreach(var btn in type.BaseTypes)
			{
				if (!Types.TryGetValue(btn, out var bt))
					continue;
				if (bt?.Properties?.Any(p => p.Name == Name) ?? false)
					return true;
				if (IsOverrideProperty(Name, bt))
					return true;
			}
			return false;
		}
		void BuildClassType(SF.Sys.Metadata.Models.Type t)
		{
			AddFile(to_java_type(t.Name), (sb) =>
			{
				var imports = new HashSet<string>();

				if (t.BaseTypes!=null)
					foreach (var bt in t.BaseTypes)
						imports.Add(bt);

				if (t.Properties!=null)
					foreach (var p in t.Properties)
						imports.Add(p.Type);
				foreach (var import in imports)
					TryImport(sb, import);

				/**
				*  请求名
				* @param params    实体类提交
				* @return          post请求json方式提交
				*/
				sb.AppendLine($"/**");
				sb.AppendLine($"* {t.Title}");
				sb.AppendLine($"* {t.Description}");
				sb.AppendLine($"*/");
				sb.Append($"public class {to_java_type(t.Name)}");
				if (t.BaseTypes != null)
				{
					sb.Append($" extends {string.Join(",", t.BaseTypes.Select(bt => to_java_type(bt)))}");
				}
				sb.AppendLine(" {");
				if (t.Properties != null)
					foreach (var p in t.Properties)
					{
						if (IsOverrideProperty(p.Name, t))
							continue;

						sb.AppendLine($"\t/**");
						sb.AppendLine($"\t* {p.Title} {(p.Optional?"[可选]":"")}");
						sb.AppendLine($"\t* {p.Description}");
						sb.AppendLine($"\t* 类型:{p.Type}");
						sb.AppendLine($"\t*/");
						sb.AppendLine($"\tpublic {to_java_type(p.Type)} {p.Name};");
					}
				sb.AppendLine("}");
			});
		}

		void BuildType(SF.Sys.Metadata.Models.Type t)
		{
			if (!t.IsInterface  && ( t.BaseTypes == null && t.Properties == null) || t.IsArrayType)
				return;

			if (t.IsEnumType)
			{
				BuildEnumType(t);
				return;
			}
			else if (t.IsDictType)
			{
				BuildDictType(t);
				return;
			}
			BuildClassType(t);
		}
		
		void BuildMethod(StringBuilder sb, Metadata.Service service, Metadata.Method method)
		{
			//if (!action.HttpMethods.Contains("Post") && !action.HttpMethods.Contains("Get"))
			//	return;
			sb.AppendLine($"/**");
			sb.AppendLine($"* {method.Title}");
			sb.AppendLine($"* {method.Description}");
			if (method.Parameters != null)
				foreach (var p in method.Parameters)
					sb.AppendLine($"* @param {p.Name} {p.Title} {(p.Optional?"[可选] ":"")}{p.Description}");
			if(method.Type!=null && Types.TryGetValue(method.Type,out SF.Sys.Metadata.Models.Type rt))
				sb.AppendLine($"* @return {rt.Title} {rt.Description}");

			sb.AppendLine($"*/");
			sb.AppendLine($"@{(method.HeavyParameter!=null ? "POST" : "GET")}(\"{service.Name}/{method.Name}\")");
			sb.AppendLine($"Observable<{to_java_type(method.Type,true)}> {method.Name}(");
			if (method.Parameters != null)
			{
				sb.AppendLine(
					method.Parameters.Select(p =>
					$"\t@{(method.HeavyParameter==p.Name ?"Body":$"Query(\"{p.Name}\")")} {to_java_type(p.Type)} {p.Name}"
					).Join(",\n")
				);
			}
			//sb.AppendLine("\tICallOptions __opts");
			sb.AppendLine($"\t);");
		}
		void BuildService(Metadata.Service service)
		{
			AddFile(service.Name , (sb) =>
			{
				var methods = service.Methods.Where(a => ActionFilter(service, a)).ToArray();
				if (methods.Length == 0)
					return;

				var imports = new HashSet<string>();
				foreach (var m in methods)
				{
					imports.Add(m.Type);
					if(m.Parameters!=null)
						foreach (var p in m.Parameters)
							imports.Add(p.Type);
				}
				foreach (var import in imports)
					TryImport(sb, import);

				sb.AppendLine($"/**");
				sb.AppendLine($"* {service.Title}");
				sb.AppendLine($"* {service.Description}");
				sb.AppendLine($"*/");
				sb.AppendLine($"public interface {service.Name} {{");
				foreach (var a in methods)
				{
					BuildMethod(sb,service, a);
				}
				sb.AppendLine("}");
			});
		}

		public IContent Build(Metadata.Library Library)
		{
			Types = Library.Types.ToDictionary(t => t.Name);
			using (var stream = new System.IO.MemoryStream())
			{
				using (Archive = new System.IO.Compression.ZipArchive(stream, System.IO.Compression.ZipArchiveMode.Create))
				{
					//foreach (var t in Library.Types)
					//	BuildType(t);
					foreach (var c in Library.Services)
						BuildService(c);

				}
				return new ByteArrayContent()
				{
					ContentType = "application/zip",
					FileName = "api.zip",
					Data = stream.ToArray()
				};
			}
		}

	}

}
