using System;
using Newtonsoft.Json;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using SF.Reflection;
namespace SF.Services.Management.Internal
{
	class ServiceCreateParameterTemplate : IServiceCreateParameterTemplate
	{
		public IReadOnlyList<object> Args { get; private set; }
		public IReadOnlyDictionary<string, string> ServiceIdents { get; private set; }

		class ServiceConverter : JsonConverter
		{
			public IServiceDetector ServiceDetector { get; set; }
			public Dictionary<string, string> ServiceIdents { get; } = new Dictionary<string, string>();
			public override bool CanConvert(Type objectType)
			{
				return ServiceDetector.IsServiceType(objectType);
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				ServiceIdents.Add(reader.Path, (string)reader.Value);
				return null;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}

		

		public static ServiceCreateParameterTemplate Load(
			ConstructorInfo ci,
			string Config,
			IServiceDetector ServiceDetector
			)
		{

			object[] args;
			var parameters = ci.GetParameters();
			IReadOnlyDictionary<string, string> sis;
			if (string.IsNullOrWhiteSpace(Config) || Config.Trim()=="null")
			{
				sis = new Dictionary<string, string>();
				args = new object[parameters.Length];
			}
			else
			{
				var sc = new ServiceConverter()
				{
					ServiceDetector = ServiceDetector
				};
				var setting = new JsonSerializerSettings
				{
					Converters = new[]
					{
						sc
					}
				};
				using (var jr = new JsonTextReader(new System.IO.StringReader(Config)))
				{
					bool SkipComment()
					{
						for (;;)
						{
							if (!jr.Read())
								return false;
							if (jr.TokenType == JsonToken.Comment)
								continue;
							return true;
						}
					}
					void ReadToObjectStart()
					{
						if (!SkipComment())
							throw new InvalidOperationException("服务配置数据格式错误");
						if (jr.TokenType != JsonToken.StartObject)
							throw new InvalidOperationException("服务配置必须是一个JSON对象");
					}
					string ReadPropertyName()
					{
						if (!SkipComment())
							throw new InvalidOperationException("服务配置数据格式错误");
						if (jr.TokenType == JsonToken.EndObject)
							return null;
						if (jr.TokenType != JsonToken.PropertyName)
							throw new InvalidOperationException("服务配置必须是一个JSON对象");
						return (string)jr.Value;
					}
					void Skip()
					{
						SkipComment();
						if (jr.TokenType == JsonToken.StartArray || jr.TokenType == JsonToken.StartConstructor || jr.TokenType == JsonToken.StartObject)
							jr.Skip();
					}
					ReadToObjectStart();
					var pdic = parameters.ToDictionary(p => p.Name);
					var serializer = JsonSerializer.Create(setting);
					var adic = new Dictionary<string, object>();
					for (;;)
					{
						var prop = ReadPropertyName();
						if (prop == null)
							break;
						if (pdic.TryGetValue(prop, out var p))
						{
							var st = p.ParameterType.IsInterfaceType() ? ServiceDetector.GetServiceType(p.ParameterType) : ServiceType.Unknown;
							if (st == ServiceType.Normal)
							{
								Skip();
								continue;
							}
							if (st == ServiceType.Managed)
							{
								SkipComment();
								sc.ServiceIdents.Add(prop, (string)jr.Value);
								continue;
							}
							SkipComment();
							adic[prop] = serializer.Deserialize(jr, p.ParameterType);
						}
						else
						{
							Skip();
						}
					}
					args = parameters.Select(p => { adic.TryGetValue(p.Name, out var a); return a; }).ToArray();
					sis = sc.ServiceIdents;
				}
			}

			return new ServiceCreateParameterTemplate
			{
				Args = args,
				ServiceIdents = sis
			};
		}

		public object GetArgument(int Index)
		{
			return Args[Index];
		}

		public string GetServiceIdent(string Path)
		{
			ServiceIdents.TryGetValue(Path, out var id);
			return id;
		}
	}
}
