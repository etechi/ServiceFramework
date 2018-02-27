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

	public class TSDBuilder
    {
        Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }
		Dictionary<string, SF.Sys.Metadata.Models.Type> Types;
		public string ApiName { get; set; }
		public string ResultFieldName { get; set; }
		StringBuilder sb {get;} = new StringBuilder();
        public TSDBuilder(string RootNamespace, string ResultFieldName,Func<Metadata.Service, Metadata.Method, bool> ActionFilter)
		{
            this.ActionFilter = ActionFilter;
			this.ApiName = RootNamespace;
			this.ResultFieldName = ResultFieldName ?? "data";
		}
		static string to_js_type(string type)
		{
			if (type.EndsWith("?"))
				return to_js_type(type.Substring(0, type.Length - 1)) + "|null";

			var i = type.IndexOf('[');
			if (i != -1)
				return to_js_type(type.Substring(0, i)) + type.Substring(i);

			i = type.IndexOf('{');
			if (i != -1)
				return $"{{[index:string]:{to_js_type(type.Substring(0, i))}}}";

			i = type.IndexOf('<');
			if (i != -1)
				return type.Replace('.', '$').Replace('<', '$').Replace(',', '_').Replace('>', '$');

			
			switch (type)
			{
				case "string": return "string";
				case "object":return "any";
				case "datetime": return "string";
                case "timespan":return "string";
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
					return "number";
				case "bool":
					return "boolean";
				case "void":
					return "void";
				case "unknown":
					return "any";
				default:
					return type.Replace('.', '$').Replace('+','$');
			}
		}
		void BuildEnumType(SF.Sys.Metadata.Models.Type t)
		{
			sb.AppendLine($"/**");
			sb.AppendLine($"* {t.Title}");
			sb.AppendLine($"* {t.Description}");
			sb.AppendLine($"*/");
			sb.AppendLine($"type {to_js_type(t.Name)} = {(t.Properties.Length==0?"string": string.Join("|",t.Properties.Select(p => $"'{p.Name}'")))};");
			sb.AppendLine($"const {to_js_type(t.Name)}Names={{");
			foreach (var p in t.Properties)
			{
				sb.AppendLine($"\t\"{p.Name}\":\"{p.Title ?? p.Name}\",");
			}
			sb.AppendLine("}");
		}
		void BuildDictType(SF.Sys.Metadata.Models.Type t)
		{
			//sb.AppendLine($"// {t.Title}");
			//sb.AppendLine($"export interface {to_js_type(t.Name)} = {{[index:string]:{to_js_type(t.ElementType)}}};");
		}
		void BuildClassType(SF.Sys.Metadata.Models.Type t)
		{
			sb.AppendLine($"/**");
			sb.AppendLine($"* {t.Title}");
			sb.AppendLine($"* {t.Description}");
			sb.AppendLine($"*/");
			sb.Append($"interface {to_js_type(t.Name)}");
			if (t.BaseTypes != null)
			{
				sb.Append($" extends {string.Join(",",t.BaseTypes.Select(bt=>to_js_type(bt)))}");
			}
			sb.AppendLine(" {");
			if (t.Properties != null)
				foreach (var p in t.Properties)
				{
					sb.AppendLine($"\t/**");
					sb.AppendLine($"\t* {p.Title} {(p.Optional ? "[可选]" : "")}");
					sb.AppendLine($"\t* {p.Description}");
					sb.AppendLine($"\t* 类型:{p.Type}");
					sb.AppendLine($"\t*/");
					sb.Append($"\t{p.Name}");
					if (p.Optional)
						sb.Append("?");
					sb.AppendLine($": {to_js_type(p.Type)};");
				}
			sb.AppendLine("}");
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
		void BuildMethod(Metadata.Service service, Metadata.Method method)
		{
			//if (!action.HttpMethods.Contains("Post") && !action.HttpMethods.Contains("Get"))
			//	return;
			sb.AppendLine($"/**");
			sb.AppendLine($"* {method.Title}");
			sb.AppendLine($"* {method.Description}");
			if (method.Parameters != null)
				foreach (var p in method.Parameters)
					sb.AppendLine($"* @param {p.Name} {p.Title} {(p.Optional ? "[可选] " : "")}{p.Description}");
			if (method.Type != null && Types.TryGetValue(method.Type, out SF.Sys.Metadata.Models.Type rt))
				sb.AppendLine($"* @return {rt.Title} {rt.Description}");

			sb.AppendLine($"*/");
			sb.AppendLine($" {method.Name}(");
			if (method.Parameters != null)
			{
				sb.AppendLine(
					method.Parameters.Select(p =>
					$"\t{p.Name}{(p.Optional?"?":"")}:{to_js_type(p.Type)}"
					).Join(",\n")
				);
			}
			sb.AppendLine($"\t) : PromiseLike<IResult<{to_js_type(method.Type)}>>;");
		}
		void BuildService(Metadata.Service service)
		{
            var methods = service.Methods.Where(a => ActionFilter(service,a)).ToArray();
            if (methods.Length == 0)
                return;
			sb.AppendLine($"/**");
			sb.AppendLine($"* {service.Title}");
			sb.AppendLine($"* {service.Description}");
			sb.AppendLine($"*/");
			sb.AppendLine($"interface I{service.Name} {{");
			foreach (var a in methods)
			{
				BuildMethod(service, a);
			}
			sb.AppendLine("}");
		}

		public string Build(Metadata.Library Library)
		{
			Types = Library.Types.ToDictionary(t => t.Name);
			foreach (var t in Library.Types)
				BuildType(t);
			foreach (var c in Library.Services)
				BuildService(c);

			sb.AppendLine($"interface I{ApiName}{{");
			foreach (var s in Library.Services)
			{
				sb.AppendLine($"/**");
				sb.AppendLine($"* {s.Title}");
				sb.AppendLine($"* {s.Description}");
				sb.AppendLine($"*/");
				sb.AppendLine($"{s.Name} : I{s.Name};");
			}
			sb.AppendLine("}");

			sb.AppendLine($"interface IResult<T>{{ {ResultFieldName}? :T }}");
			sb.AppendLine($"export const {ApiName}:I{ApiName};");
			sb.AppendLine("export function setServiceInvoker(invoker: (service: string, method: string,postArg:string, args: any)=>PromiseLike<any>);");
			return sb.ToString();

		}
	}

}
