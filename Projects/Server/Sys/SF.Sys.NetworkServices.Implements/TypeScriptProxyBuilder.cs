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
using System.Text;

namespace SF.Sys.NetworkService
{

	public class TypeScriptProxyBuilder
    {

        Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }

        StringBuilder sb{get;} = new StringBuilder();
        public TypeScriptProxyBuilder(Func<Metadata.Service, Metadata.Method, bool> ActionFilter)
        {
            this.ActionFilter = ActionFilter;
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
			sb.AppendLine($"// {t.Title}");
			sb.AppendLine($"export type {to_js_type(t.Name)} = {string.Join("|",t.Properties.Select(p => $"'{p.Name}'"))};");
			sb.AppendLine($"export const {to_js_type(t.Name)}Names={{");
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
			sb.AppendLine($"// {t.Title}");
			sb.Append($"export interface {to_js_type(t.Name)}");
			if (t.BaseTypes != null)
			{
				sb.Append($" extends {string.Join(",",t.BaseTypes.Select(bt=>to_js_type(bt)))}");
			}
			sb.AppendLine(" {");
			if (t.Properties != null)
				foreach (var p in t.Properties)
				{
					sb.AppendLine($"\t//{p.Title}");
					sb.AppendLine($"\t//类型:{p.Type}");
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

			var za = new System.IO.Compression.ZipArchive(null,System.IO.Compression.ZipArchiveMode.Create);
			za.CreateEntry("aa").Open();

		}
		void BuildMethod(Metadata.Service service, Metadata.Method method)
		{
			//if (!action.HttpMethods.Contains("Post") && !action.HttpMethods.Contains("Get"))
			//	return;
			sb.AppendLine($"//{method.Title}");
			sb.AppendLine($"//{method.Description}");
			sb.AppendLine($"{method.Name}(");
			if (method.Parameters != null)
			{
				foreach (var p in method.Parameters)
				{	
					sb.AppendLine($"\t//{p.Title}");
					sb.AppendLine($"\t//类型:{p.Type}");
					sb.Append($"\t{p.Name}");
					if (p.Optional) sb.Append("?");
					sb.Append($": {to_js_type(p.Type)}");
					sb.AppendLine(",");
				}
			}
			sb.AppendLine("\t__opts?:ICallOptions");
			sb.AppendLine($"\t) : PromiseLike<{to_js_type(method.Type)}> {{");
			sb.AppendLine($"\treturn _invoker(\n\t\t'{service.Name}',\n\t\t'{method.Name}',");
			if (method.Parameters != null && method.Parameters.Cast<SF.Sys.Metadata.Models.Parameter>().Any(p => p.Name!=method.HeavyParameter))
			{
				sb.AppendLine("\t\t{");
				sb.Append("\t\t\t" + 
					string.Join(",\n\t\t\t",
					method.Parameters.Cast<SF.Sys.Metadata.Models.Parameter>()
					.Where(p => p.Name!=method.HeavyParameter)
					.Select(p => $"{p.Name}:{p.Name}"))
					);
				sb.AppendLine();
				sb.AppendLine("\t\t},");
			}
			else
				sb.AppendLine("\t\tnull,");
			if (method.HeavyParameter!=null)
			{
				sb.AppendLine($"\t\t{method.Parameters.Cast<SF.Sys.Metadata.Models.Parameter>().Where(p => p.Name==method.HeavyParameter).Single().Name},");
			}
			else
				sb.AppendLine("\t\tnull,");
			sb.AppendLine("\t\t__opts");
			sb.AppendLine("\t\t);");
			sb.AppendLine("},");
		}
		void BuildService(Metadata.Service service)
		{
            var methods = service.Methods.Where(a => ActionFilter(service,a)).ToArray();
            if (methods.Length == 0)
                return;
			sb.AppendLine($"//{service.Title}");
			sb.AppendLine($"//{service.Description}");
			sb.AppendLine($"export const {service.Name}={{");
			foreach (var a in methods)
			{
				BuildMethod(service, a);
			}
			sb.AppendLine("}");
		}

		public string Build(Metadata.Library Library)
		{
			sb.AppendLine(@"
export interface IQueryPaging {
    offset?: number;
    limit?: number;
    sortMethod?: string;
    sortOrder?: ""Asc"" | ""Desc"";

	totalRequired ?: boolean;
	summaryRequired ?: boolean;
}
export interface ICallOptions
{
	paging?: IQueryPaging,
	query?:any,
	serviceId?:number
}
export interface IApiInvoker{
	(type: string,method: string,query: { [index: string]: any},post: { [index: string]: any}, opts?: ICallOptions) :any
}

var _invoker:IApiInvoker=null;
export function setApiInvoker(invoker:IApiInvoker){
	_invoker=invoker;
}

");
			foreach (var t in Library.Types)
				BuildType(t);
			foreach (var c in Library.Services)
				BuildService(c);
			return sb.ToString();
		}
	}

}
