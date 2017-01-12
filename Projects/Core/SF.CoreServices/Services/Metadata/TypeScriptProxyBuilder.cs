using System;
using System.Linq;
using System.Text;

namespace SF.Services.Metadata
{

	public class TypeScriptProxyBuilder
    {

        Func<Models.Service, Models.Method, bool> ActionFilter { get; }

        StringBuilder sb{get;} = new StringBuilder();
        public TypeScriptProxyBuilder(Func<Models.Service, Models.Method, bool> ActionFilter)
        {
            this.ActionFilter = ActionFilter;
        }
		static string to_js_type(string type)
		{
			var i = type.IndexOf('[');
			if (i != -1)
				return to_js_type(type.Substring(0, i)) + type.Substring(i);

			i = type.IndexOf('<');
			if (i != -1)
				return $"{{[index:string]:{to_js_type(type.Substring(0, i))}}}";

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
		void BuildEnumType(SF.Metadata.Models.Type t)
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
		void BuildDictType(SF.Metadata.Models.Type t)
		{
			//sb.AppendLine($"// {t.Title}");
			//sb.AppendLine($"export interface {to_js_type(t.Name)} = {{[index:string]:{to_js_type(t.ElementType)}}};");
		}
		void BuildClassType(SF.Metadata.Models.Type t)
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
		void BuildType(SF.Metadata.Models.Type t)
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
		void BuildMethod(Models.Service service,Models.Method method)
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
			if (method.Parameters != null && method.Parameters.Cast<SF.Metadata.Models.Parameter>().Any(p => p.Name!=method.HeavyParameter))
			{
				sb.AppendLine("\t\t{");
				sb.Append("\t\t\t" + 
					string.Join(",\n\t\t\t",
					method.Parameters.Cast<SF.Metadata.Models.Parameter>()
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
				sb.AppendLine($"\t\t{method.Parameters.Cast<SF.Metadata.Models.Parameter>().Where(p => p.Name==method.HeavyParameter).Single().Name},");
			}
			else
				sb.AppendLine("\t\tnull,");
			sb.AppendLine("\t\t__opts");
			sb.AppendLine("\t\t);");
			sb.AppendLine("},");
		}
		void BuildService(Metadata.Models.Service service)
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

		public string Build(Metadata.Models.Library Library)
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
	query?:any
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
