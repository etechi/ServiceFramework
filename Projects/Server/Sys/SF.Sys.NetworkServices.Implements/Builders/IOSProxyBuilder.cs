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
		public bool MergeBaseType { get; set; }
		public IOSProxyBuilder(
			string CommonImports,
			string BaseService,
			bool MergeBaseType,
			Func<Metadata.Service, Metadata.Method, bool> ActionFilter
			)
		{
			this.MergeBaseType = MergeBaseType;
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
		bool TryImport(StringBuilder sb, string type,bool import)
		{
			if (Imported.Add(type) && Types.TryGetValue(type, out var st))
				BuildType(st);

			if (type.EndsWith("?"))
				return TryImport(sb, type.Substring(0, type.Length - 1), import);

			var i = type.IndexOf('[');
			if (i != -1)
				return TryImport(sb, type.Substring(0, i), import);

			i = type.IndexOf('{');
			if (i != -1)
				return TryImport(sb, type.Substring(0, i), import);

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
			if(import)
				sb.AppendLine($"#import \"{type.Replace('.', '_').Replace('+', '_')}.h\"");
			else
				sb.AppendLine($"@class {type.Replace('.', '_').Replace('+', '_')};");
			return true;
		}
		string getPropType(string type)
		{
			switch (type)
			{
				case "string": 
				case "datetime": 
				case "timespan": return "copy";
				case "long":
				case "ulong":
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
				case "decimal":
				case "float":
				case "double":
				case "bool":
					return "assign";
				case "void":
				case "object":
				case "unknown":
				default:
					return "strong";
			}
		}

		(string name,string format,string value) argFormat(string type,string arg)
		{
			switch (type)
			{
				case "string":
				case "datetime":
				case "timespan":
					return (arg,"%@", $"[{arg} stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]]");
				case "long":
				case "ulong":
					return (arg, "%ld", arg);
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
					return (arg,"%d", arg);
				case "decimal":
				case "float":
				case "double":
					return (arg, "%f", arg);
				case "bool":
					return (arg, "%@", $"{arg}==0?@\"true\":@\"false\"");
				case "void":
				case "object":
				case "unknown":
				default:
					return (arg, "%@",$"[[NSString stringWithFormat:@\"%@\",{arg}] stringByAddingPercentEncodingWithAllowedCharacters:[NSCharacterSet URLQueryAllowedCharacterSet]]");
			}
		}
		(string process,string result) resolveResult(string type,string arg)
		{
			if (type.EndsWith("[]"))
			{
				var eleType = type.Substring(0, type.Length - 2);
				var eleCode = resolveResult(eleType, "are[i]");
				return ($@"
	NSArray *are = (NSArray*)re;
    NSMutableArray<{to_ios_type(eleType, true)}> *arr = [NSMutableArray new];
    for (int i = 0; i<are.count; i++) {{
		{eleCode.process??""}
		[arr addObject: {eleCode.result}];
    }}
", "arr");
			}
			switch (type)
			{
				case "string": 
				case "datetime": 
				case "timespan": return (null,$"(NSString *){arg}");
				case "long":
				case "ulong": return (null, $"[((NSNumber*){arg}) longValue]");
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char": return (null, $"[((NSNumber*){arg}) intValue]");
				case "decimal": return (null, $"[((NSNumber*){arg}) doubleValue]");
				case "float": return (null, $"[((NSNumber*){arg}) floatValue]");
				case "double": return (null, $"[((NSNumber*){arg}) doubleValue]");
				case "bool": return (null, $"[((NSNumber*){arg}) intValue]!=0");
				case "void": return (null, "");
				case "object":
				case "unknown": return (null, arg);
				default:
					return (null, $"[{to_ios_type(type, false)} mj_objectWithKeyValues:(NSDictionary *){arg}]");
			}
		}
		static string EscName(string name)
		{
			if (name == "id") return "_id";
			return name;
		}
		string to_ios_type(string type, bool withPointer, bool EscapeEnumName = true, bool objectMode = false)
		{

			if (type.EndsWith("?"))
				return to_ios_type(type.Substring(0, type.Length - 1), withPointer, EscapeEnumName, true);

			var i = type.IndexOf('[');
			if (i != -1)
				return "NSArray<"+to_ios_type(type.Substring(0, i), true,true,true) + ">" + (withPointer?" *":"");

			i = type.IndexOf('{');
			if (i != -1)
			{
				var et = to_ios_type(type.Substring(0, i), true,true,true);
				if (et == "NSString *")
					et = "NSDictionary";
				else
					et = "NSDictionary<NSString*," + et + ">";
				return et + (withPointer ? " *" : "");
			}

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
				case "ulong": return objectMode?"NSNumber"+ (withPointer ? " *" : ""): "long" ;
				case "int":
				case "short":
				case "sbyte":
				case "uint":
				case "ushort":
				case "byte":
				case "char":
					return objectMode ? "NSNumber" + (withPointer ? " *" : "") : "int" ;
				case "float":
					return objectMode ? "NSNumber" + (withPointer ? " *" : "") : "float";
				case "decimal":
				case "double":
					return objectMode ? "NSNumber" + (withPointer ? " *" : "") : "double";
				case "bool":
					return objectMode ? "NSNumber" + (withPointer ? " *" : "") : "BOOL" ;
				case "void":
				case "object":
				case "unknown":
					return "NSObject" + (withPointer ? " *" : "");
				default:
					return type.Replace('.', '_').Replace('+', '_') + (withPointer ? " *" : "");
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
			var type = to_ios_type(t.Name, false, false);
			AddFile(type + ".h", (sb) =>
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
						sb.AppendLine($"#define  {type}_{p.Name} (\"{p.Name}\")");
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
			var imports = new Dictionary<string, bool>();

			IEnumerable<(Sys.Metadata.Models.Type type, Sys.Metadata.Models.Property prop)> props;
			if (MergeBaseType)
			{
				props = ADT.Tree.AsEnumerable(
					t,
					ti => ti.BaseTypes?.Select(bt => Types[bt])
					).Reverse()
					.Where(ti => ti.Properties != null)
					.SelectMany(ti => ti.Properties.Select(p => (type: ti, prop: p)));
			}
			else
				props = t.Properties.Select(p => (type: t, prop: p))
					?? Enumerable.Empty<(Sys.Metadata.Models.Type type, Sys.Metadata.Models.Property prop)>();

			props = props.Where(p => !IsOverrideProperty(p.prop.Name, p.type));

			foreach (var p in props)
				imports[p.prop.Type] = false;


			if (t.BaseTypes != null && !MergeBaseType)
				foreach (var bt in t.BaseTypes)
					imports[bt] = true;

			AddFile(to_ios_type(t.Name, false, false) + ".h", (sb) =>
				{

					foreach (var import in imports.OrderBy(p => p.Value ? 0 : 1))
						TryImport(sb, import.Key, import.Value);


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
					if (t.BaseTypes != null && !MergeBaseType)
					{
						sb.AppendLine($" : {string.Join(",", t.BaseTypes.Select(bt => to_ios_type(bt, false, false)))}");
					}
					else
					{
						sb.AppendLine($" : JSONModel");
					}



					foreach (var p in props)
					{
						sb.AppendLine($"\t/**");
						sb.AppendLine($"\t* {p.prop.Title} {(p.prop.Optional ? "[可选]" : "")}");
						sb.AppendLine($"\t* {p.prop.Description}");
						sb.AppendLine($"\t* 类型:{p.prop.Type}");
						sb.AppendLine($"\t*/");
						var ptype = p.prop.Type;
						if (p.prop.Optional && !ptype.EndsWith("?"))
							ptype += "?";
						sb.AppendLine($"\t@property ({getPropType(ptype)},nonatomic) {to_ios_type(ptype, true)} {p.prop.Name};");
					}
					sb.AppendLine("@end");
				});

			AddFile(to_ios_type(t.Name, false, false) + ".m", (sb) =>
			{
				imports[t.Name] = true;
				foreach (var import in imports)
					TryImport(sb, import.Key, true);
				sb.AppendLine($@"
@implementation {to_ios_type(t.Name, false, false)}
+(BOOL)propertyIsOptional:(NSString*)propertyName
{{
	return YES;
}}
{(props.Any(p=>p.prop.Type.Contains('['))?$@"
+ (NSDictionary *)objectClassInArray
{{
    // 数组名 : 模型名
    return @{{
		{props
			.Where(p=>p.prop.Type.Contains('['))
			.Select(p=> $"@\"{p.prop.Name}\": @\"{to_ios_type(p.prop.Type.Split2('[').Item1,false,true,true)}\"").Join(",")}
	}};
}}":""
)}
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
				sb.Append($":({to_ios_type(p.Type,true)}) {EscName(p.Name)} ");
				for (var i = 1; i < method.Parameters.Length; i++)
				{
					p = method.Parameters[i];
					sb.Append($"{EscName(p.Name)}:({to_ios_type(p.Type,true)}) {EscName(p.Name)} ");
				}

				sb.Append(" success");
			}

			sb.Append($" : (void(^)({(method.Type=="void"?"":$"{to_ios_type(method.Type,true)} re")}))success failure:(void(^)(NSError * error))failure");
		}


		void BuildMethod(StringBuilder sb, Metadata.Service service, Metadata.Method method)
		{
			BuildMethodPrototype(sb, service, method);

		}
		void collectAllType(HashSet<string> hash,string type)
		{
			if (!hash.Add(type))
				return;
			if (!Types.TryGetValue(type, out var t))
				return;
			if (t.BaseTypes != null && !MergeBaseType)
				foreach (var bt in t.BaseTypes)
					collectAllType(hash, bt);
			if (t.Properties != null)
				foreach (var p in t.Properties)
					collectAllType(hash, p.Type);
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
					collectAllType(imports,m.Type);
					if (m.Parameters != null)
						foreach (var p in m.Parameters)
							collectAllType(imports,p.Type);
				}
				foreach (var import in imports)
					TryImport(sb, import,true);

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
						var pairs = a.Parameters.Where(p => p.Name != a.HeavyParameter).Select(p => argFormat(p.Type, EscName(p.Name))).ToArray();

						sb.Append($"	NSString *url = [NSString stringWithFormat:@\"{service.Name}/{a.Name}?");
						sb.Append(pairs.Select(p=>$"{p.name}={p.format}").Join("&"));
						sb.Append("\"");
						sb.Append(pairs.Select(p => $"\n		,{p.value}").Join(""));
						sb.AppendLine("\t];");
					}
					else
						sb.AppendLine($"	NSString *url = @\"{service.Name}/{a.Name}\";");
					if (a.HeavyParameter!=null)
						sb.AppendLine($"	NSDictionary *dict = [self dictionaryWithModel: {a.HeavyParameter}];");

					if (a.HeavyParameter == null)
						sb.AppendLine($"[self getUrlPath:url success:^(NSObject *re) {{");
					else
						sb.AppendLine($"[self postUrlPath:url withParamers:dict success:^(NSObject *re) {{");

					var code = resolveResult(a.Type, "re");
					sb.AppendLine(
					$@"
		{code.process}
        success({code.result});
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
