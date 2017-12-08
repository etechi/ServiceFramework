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

	public class IOSProxyBuilder
	{
		Func<Metadata.Service, Metadata.Method, bool> ActionFilter { get; }
		Dictionary<string, SF.Sys.Metadata.Models.Type> Types;
		System.IO.Compression.ZipArchive Archive;
		public string BaseService { get; set; }
		public string CommonImports { get; set; }
		public IOSProxyBuilder(
			string CommonImports,
			string BaseService,
			Func<Metadata.Service, Metadata.Method, bool> ActionFilter
			)
		{
			this.CommonImports = CommonImports;
			this.BaseService = BaseService;
			this.ActionFilter = ActionFilter;
		}
		bool IsEnumType(string type)
		{
			if (Types.TryGetValue(type, out var t))
				return t.IsEnumType;
			return false;
		}
		HashSet<string> Imported { get; } = new HashSet<string>(); 
		bool TryImport(StringBuilder sb, string type)
		{
			if (Imported.Add(type) && Types.TryGetValue(type, out var st))
				BuildType(st);



			if (type.EndsWith("?"))
				return TryImport(sb, type.Substring(0, type.Length - 1));

			var i = type.IndexOf('[');
			if (i != -1)
				return TryImport(sb, type.Substring(0, i));

			i = type.IndexOf('{');
			if (i != -1)
				return TryImport(sb, type.Substring(0, i));

			if (IsEnumType(type))
				return false;
			i = type.IndexOf('<');
			if (i != -1)
				type = type.Replace('.', '_').Replace('<', '_').Replace(',', '_').Replace('>', '_');

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
			sb.AppendLine($"#import \"{type.Replace('.', '_').Replace('+', '_')}.h\"");
			return true;
		}
		string to_ios_type(string type, bool withPointer, bool EscapeEnumName = true)
		{

			if (type.EndsWith("?"))
				return to_ios_type(type.Substring(0, type.Length - 1), withPointer);

			var i = type.IndexOf('[');
			if (i != -1)
				return "NSArray<"+to_ios_type(type.Substring(0, i), false) + ">" + (withPointer?" *":"");

			i = type.IndexOf('{');
			if (i != -1)
				return "NSDictionary<NSString," + to_ios_type(type.Substring(0, i), false) + ">" + (withPointer ? " *" : "");

			if (EscapeEnumName && IsEnumType(type))
				return "NSString" +(withPointer ? " *" : "");

			i = type.IndexOf('<');
			if (i != -1)
				return type.Replace('.', '_').Replace('<', '_').Replace(',', '_').Replace('>', '_') + (withPointer ? " *" : "");


			switch (type)
			{
				case "string": return "NSString" + (withPointer ? " *" : "");
				case "datetime": return "NSString" + (withPointer ? " *" : "");
				case "timespan": return "NSString" + (withPointer ? " *" : "");
				case "long":
				case "ulong": return "NSNumber" + (withPointer ? " *" : "");
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
					return "NSInteger" +  (withPointer ? " *" : "");
				case "decimal":
				case "float":
				case "double":
					return "NSNumber" + (withPointer ? " *" : "");
				case "bool":
					return "NSInteger" + (withPointer ? " *" : "");
				case "void":
				case "object":
				case "unknown":
					return "NSObject<Optional>";
				default:
					return type.Replace('.', '_').Replace('+', '_');
			}
		}
		void AddFile(string name, Action<StringBuilder> content)
		{
			var sb = new StringBuilder();
			sb.AppendLine(CommonImports);
			var l = sb.Length;
			content(sb);
			if (l == sb.Length)
				return;
			var buf = Encoding.UTF8.GetBytes(sb.ToString());

			using (var s = Archive.CreateEntry(name).Open())
			{
				
				s.Write(buf, 0, buf.Length);
			}
		}
		void BuildEnumType(SF.Sys.Metadata.Models.Type t)
		{
			AddFile(to_ios_type(t.Name + ".h", false, false), (sb) =>
				{
					sb.AppendLine($"/**");
					sb.AppendLine($"* {t.Title}");
					sb.AppendLine($"* {t.Description}");
					sb.AppendLine($"*/");
					foreach (var p in t.Properties)
					{
						sb.AppendLine($"/**");
						sb.AppendLine($"* {t.Title}");
						sb.AppendLine($"* {t.Description}");
						sb.AppendLine($"*/");
						sb.AppendLine($"#define {p.Name} (\"{p.Name}\")");
					}
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
			foreach (var btn in type.BaseTypes)
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
			AddFile(to_ios_type(t.Name, false, false) + ".h", (sb) =>
				{
					var imports = new HashSet<string>();

					if (t.BaseTypes != null)
						foreach (var bt in t.BaseTypes)
							imports.Add(bt);

					if (t.Properties != null)
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
					sb.Append($"@interface {to_ios_type(t.Name, false, false)}");
					if (t.BaseTypes != null)
					{
						sb.AppendLine($" : {string.Join(",", t.BaseTypes.Select(bt => to_ios_type(bt, false, false)))}");
					}
					else
					{
						sb.AppendLine($" : JSONModel");
					}
					if (t.Properties != null)
						foreach (var p in t.Properties)
						{
							if (IsOverrideProperty(p.Name, t))
								continue;

							sb.AppendLine($"\t/**");
							sb.AppendLine($"\t* {p.Title} {(p.Optional ? "[可选]" : "")}");
							sb.AppendLine($"\t* {p.Description}");
							sb.AppendLine($"\t* 类型:{p.Type}");
							sb.AppendLine($"\t*/");
							sb.AppendLine($"\t@property (copy,nonatomic) {to_ios_type(p.Type, false, false)} *{p.Name};");
						}
					sb.AppendLine("@end");
				});

			AddFile(to_ios_type(t.Name, false, false) + ".m", (sb) =>
			{
				TryImport(sb,t.Name);
				sb.AppendLine($@"
@implementation {to_ios_type(t.Name, false, false)}
+(BOOL)propertyIsOptional:(NSString*)propertyName
{{
	return YES;
}}
@end
");
			});
		}

		void BuildType(SF.Sys.Metadata.Models.Type t)
		{
			if (!t.IsInterface && (t.BaseTypes == null && t.Properties == null) || t.IsArrayType)
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
		void BuildMethodPrototype(StringBuilder sb, Metadata.Service service, Metadata.Method method)
		{
			sb.AppendLine($"/**");
			sb.AppendLine($"* {method.Title}");
			sb.AppendLine($"* {method.Description}");
			if (method.Parameters != null)
				foreach (var p in method.Parameters)
					sb.AppendLine($"* @param {p.Name} {p.Title} {(p.Optional ? "[可选] " : "")}{p.Description}");
			if (method.Type != null && Types.TryGetValue(method.Type, out SF.Sys.Metadata.Models.Type rt))
				sb.AppendLine($"* @return {rt.Title} {rt.Description}");

			sb.AppendLine($"*/");
			sb.Append($"+ (void) {method.Name}");
			if ((method.Parameters?.Length??0) > 0)
			{
				var p = method.Parameters[0];
				sb.Append($":({to_ios_type(p.Type,true)}) {p.Name} ");
				for (var i = 1; i < method.Parameters.Length; i++)
				{
					p = method.Parameters[0];
					sb.Append($"{p.Name}:({to_ios_type(p.Type,true)}) {p.Name} ");
				}
			}

			sb.Append($" success: (void(^)({to_ios_type(method.Type,true)} responseObject))success failure:(void(^)(NSError * error))failure");
		}


		void BuildMethod(StringBuilder sb, Metadata.Service service, Metadata.Method method)
		{
			BuildMethodPrototype(sb, service, method);

		}
		void BuildService(Metadata.Service service)
		{
			AddFile(service.Name + ".h", (sb) =>
			{
				var methods = service.Methods.Where(a => ActionFilter(service, a)).ToArray();
				if (methods.Length == 0)
					return;

				var imports = new HashSet<string>();
				foreach (var m in methods)
				{
					imports.Add(m.Type);
					if (m.Parameters != null)
						foreach (var p in m.Parameters)
							imports.Add(p.Type);
				}
				foreach (var import in imports)
					TryImport(sb, import);

				sb.AppendLine($"/**");
				sb.AppendLine($"* {service.Title}");
				sb.AppendLine($"* {service.Description}");
				sb.AppendLine($"*/");
				sb.AppendLine($"@interface {service.Name} : {BaseService}");
				foreach (var a in methods)
				{
					BuildMethodPrototype(sb, service, a);
					sb.AppendLine(";");
				}
				sb.AppendLine("@end");
			});
			AddFile(service.Name + ".m", (sb) =>
			{
				 var methods = service.Methods.Where(a => ActionFilter(service, a)).ToArray();
				 if (methods.Length == 0)
					 return;

				 var imports = new HashSet<string>();
				 foreach (var m in methods)
				 {
					 imports.Add(m.Type);
					 if (m.Parameters != null)
						 foreach (var p in m.Parameters)
							 imports.Add(p.Type);
				 }
				 foreach (var import in imports)
					 TryImport(sb, import);

				 sb.AppendLine($"#import \"{service.Name}.h\"");

				 sb.AppendLine($"/**");
				 sb.AppendLine($"* {service.Title}");
				 sb.AppendLine($"* {service.Description}");
				 sb.AppendLine($"*/");
				 sb.AppendLine($"@implementation {service.Name} ");
				 foreach (var a in methods)
				 {
					 BuildMethodPrototype(sb, service, a);
					 sb.AppendLine("{");
					if ((a.Parameters?.Length ?? 0) > (a.HeavyParameter == null ? 0 : 1))
					{
						sb.Append($"	NSString *url = [NSString stringWithFormat:@\"{service.Name}/{a.Name}?");
						sb.Append(a.Parameters.Where(p => p.Name != a.HeavyParameter).Select(p=>$"{p.Name}=%@").Join("&"));
						sb.Append("\"");
						sb.Append(
							a.Parameters.Where(p => p.Name != a.HeavyParameter)
							.Select(p =>
								p.Type=="string"? 
								$"\n		,[{p.Name} stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]]":
								$"\n		,[[NSString stringWithFormat:@\"%@\",{p.Name}] stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]]")
							.Join("")
							);
						sb.AppendLine("];");
					}
					else
						sb.AppendLine($"	NSString *url = @\"{service.Name}/{a.Name}\";");
					if (a.HeavyParameter!=null)
						sb.AppendLine($"	NSDictionary *dict = [self dictionaryWithModel: {a.HeavyParameter}];");

					if (a.HeavyParameter == null)
						sb.AppendLine($"[self getUrlPath:url success:^(NSDictionary *re) {{");
					else
						sb.AppendLine($"[self postUrlPath:url withParamers:dict success:^(NSDictionary *re) {{");

					sb.AppendLine(
					$@"
        success([[{to_ios_type(a.Type,true)} alloc]initWithDictionary:re error:nil]);
    }} failure:^(NSError *error) {{
		failure(error);
    }}];
}}
");

							}
				sb.AppendLine("@end");
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
