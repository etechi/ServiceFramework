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

	public class CSProxyBuilder
    {

        Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }
		Func<SF.Sys.Metadata.Models.Type, bool> TypeFilter { get; }
		public string Namespace { get; }

		StringBuilder sb{get;} = new StringBuilder();
        public CSProxyBuilder(
			string Namespace,
			Func<Metadata.Service, Metadata.Method, bool> ActionFilter,
			Func<SF.Sys.Metadata.Models.Type, bool> TypeFilter
			)
        {
			this.Namespace = Namespace;

			this.ActionFilter = ActionFilter;
			this.TypeFilter = TypeFilter;

		}
		string to_cs_type(string type,bool decl=false,bool option=false)
		{
			if (type.EndsWith("?"))
				return to_cs_type(type.Substring(0, type.Length - 1),true);

			var i = type.IndexOf('[');
			if (i != -1)
				return to_cs_type(type.Substring(0, i)) + type.Substring(i);

			i = type.IndexOf('{');
			if (i != -1)
				return $"Dictionary<string,{to_cs_type(type.Substring(0, i))}>";
			
			i = type.IndexOf('<');
			if (i != -1)
				return decl?type.Replace('<','_').Replace('>','_').Replace('.','_'): Namespace + "."+type;

			
			switch (type)
			{
				case "string": return "string";
				case "object":return "object";
				case "datetime": return "DateTime"+(option?"?":"");
                case "timespan":return "TimeSpan" + (option ? "?" : "");
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
					return type + (option ? "?" : "");
				case "void":
					return type;
				case "unknown":
					return "object";
				default:
					return decl?type:Namespace + "." + type;
			}
		}
		void BuildEnumType(SF.Sys.Metadata.Models.Type t)
		{
			sb.AppendLine($"/// <summary>{t.Title?.Replace("\n", "")}</summary>");
			sb.AppendLine($"public enum {to_cs_type(GetTypeName(t.Name), true)} {{");
			foreach (var p in t.Properties)
			{
				sb.AppendLine($"/// <summary>{t.Title}</summary>");
				sb.AppendLine($"\t{p.Name},");
				sb.AppendLine();
			}
			sb.AppendLine("}");
			sb.AppendLine();
		}
		void BuildDictType(SF.Sys.Metadata.Models.Type t)
		{
			//sb.AppendLine($"// {t.Title}");
			//sb.AppendLine($"export interface {to_js_type(t.Name)} = {{[index:string]:{to_js_type(t.ElementType)}}};");
		}
		
		void BuildClassType(SF.Sys.Metadata.Models.Type t)
		{
			sb.AppendLine($"/// <summary> {t.Title?.Replace("\n","")}</summary>");
			sb.Append($"public class {to_cs_type(GetTypeName(t.Name),true)}");
			var bts = t.BaseTypes?.Where(tn => Types.ContainsKey(tn))?.ToArray();
			if (bts != null && bts.Length>0)
			{
				sb.Append($" : {string.Join(",",bts.Select(bt=>to_cs_type(bt)))}");
			}
			sb.AppendLine(" {");
			if (t.Properties != null)
				foreach (var p in t.Properties)
				{
					sb.AppendLine($"\t/// <summary>{p.Title?.Replace("\n", "")}</summary>");
					sb.Append($"\tpublic {to_cs_type(p.Type)}");
					//if (p.Optional)
					//	sb.Append("?");
					sb.AppendLine($" {p.Name} {{get;set;}}");
					sb.AppendLine();
				}
			sb.AppendLine("}");
			sb.AppendLine();
		}
		void BuildType(SF.Sys.Metadata.Models.Type t)
		{
			if (!t.IsInterface  && ( t.BaseTypes == null && t.Properties == null && !t.Name.Contains('_')) || t.IsArrayType)
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
		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <param name="method"></param>
		void BuildMethod(Metadata.Service service, Metadata.Method method)
		{
			//if (!action.HttpMethods.Contains("Post") && !action.HttpMethods.Contains("Get"))
			//	return;
			sb.AppendLine($"///<summary>{method.Title?.Replace("\n", "")} {method.Description?.Replace("\n", "")}</summary>");
			if (method.Parameters != null)
				foreach (var p in method.Parameters)
					sb.AppendLine($"/// <param name=\"{p.Name}\">{p.Title}</param>");
			sb.AppendLine($"public Task{(method.Type=="void"?"":$"<{to_cs_type(method.Type)}>")} {method.Name}(");
			if (method.Parameters != null)
			{
				var first = true;
				foreach (var p in method.Parameters)
				{
					if (first)
						first = false;
					else
						sb.AppendLine(",");
					sb.Append($"\t{to_cs_type(p.Type)}");
					//if (p.Optional) sb.Append("?");
					sb.Append($" {p.Name}");
				}
			}
			sb.AppendLine($@"
				) => 
				Invoker.Invoke{(method.Type == "void" ? "" : $"<{to_cs_type(method.Type)}>")}(
					""{service.Name}"", 
					""{method.Name}"", 
					{(method.HeavyParameter==null?"null":"\""+method.HeavyParameter+"\"")}
					{method.Parameters?.Select(p=>$",(\"{p.Name}\",{p.Name})")?.Join("\n") ??""}
					);"
				);
		}

		bool BuildService(Metadata.Service service)
		{
            var methods = service.Methods.Where(a => ActionFilter(service,a)).ToArray();
            if (methods.Length == 0)
                return false;
			sb.AppendLine($"///<summary>{service.Title?.Replace("\n", "")} {service.Description?.Replace("\n", "")}</summary>");
			sb.AppendLine($@"public class {service.Name} {{
			IApiInvoker Invoker {{ get; }}
			public {service.Name}(IApiInvoker Invoker) => this.Invoker = Invoker;
");
			foreach (var a in methods)
			{
				BuildMethod(service, a);
			}
			sb.AppendLine("}");
			return true;
		}
	
		static string[] SplitTypeName(string Name)
		{
			var i = Name.IndexOf('<');
			if (i == -1) return Name.Split('.');
			return Name.Substring(0, i).Split('.').WithLast(Name.Substring(i + 1)).ToArray();
		}
		static string GetTypeName(string Name)
		{
			var i = Name.IndexOf('<');
			return Name.LastSplit2('.',i==-1?null:(int?)(i-1)).Item2;
		}
		Dictionary<string, Sys.Metadata.Models.Type> Types;
		public string Build(Metadata.Library Library)
		{
			Types = Library.Types.ToDictionary(t => t.Name);

			sb.AppendLine(@"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
");
			sb.AppendLine($"namespace {Namespace} {{");
			sb.AppendLine(@"

public interface IApiInvoker{
	Task<T> Invoke<T>(string type,string method,string heavyParam, params object[] args);
	Task Invoke(string type,string method,string heavyParam, params object[] args);
}
");
			var roots = ADT.Tree.BuildByLeafPath(Library.Types.Where(TypeFilter), t => SplitTypeName(t.Name));

			void build(ADT.Tree.NamedNode<Sys.Metadata.Models.Type> n){
				if(n.Value==null)
				{
					sb.AppendLine($"namespace {n.Name} {{");
					foreach (var c in n)
						build(c);
					sb.AppendLine("}");
					sb.AppendLine();
				}
				else
				{
					BuildType(n.Value);
				}
			}
			foreach (var n in roots)
				build(n);

			var svcs = new List<Metadata.Service>();
			foreach (var c in Library.Services)
				if (BuildService(c))
					svcs.Add(c);

			sb.AppendLine($@"

	public class Client
	{{
		{svcs.Select(s=>$@"
		/// <summary> {s.Title?.Replace("\n","")} {s.Description?.Replace("\n","")}</summary>
		public {s.Name} {s.Name} {{get;}}
").Join("\n")}
		public Client(IApiInvoker Invoker)
		{{
			{svcs.Select(s=>$"this.{s.Name} =new {s.Name}(Invoker);").Join("\n")}
		}}
	}}

");

			sb.AppendLine("}");
			return sb.ToString();
		}
	}

}
