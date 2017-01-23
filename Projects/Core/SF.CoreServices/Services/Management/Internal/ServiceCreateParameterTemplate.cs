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


		static bool SkipComment(JsonTextReader jr)
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
		static void ReadToObjectStart(JsonTextReader jr)
		{
			if (!SkipComment(jr))
				throw new InvalidOperationException("服务配置数据格式错误");
			if (jr.TokenType != JsonToken.StartObject)
				throw new InvalidOperationException("服务配置必须是一个JSON对象");
		}
		static string ReadPropertyName(JsonTextReader jr)
		{
			if (!SkipComment(jr))
				throw new InvalidOperationException("服务配置数据格式错误");
			if (jr.TokenType == JsonToken.EndObject)
				return null;
			if (jr.TokenType != JsonToken.PropertyName)
				throw new InvalidOperationException("服务配置必须是一个JSON对象");
			return (string)jr.Value;
		}
		static void Skip(JsonTextReader jr)
		{
			SkipComment(jr);
			if (jr.TokenType == JsonToken.StartArray || jr.TokenType == JsonToken.StartConstructor || jr.TokenType == JsonToken.StartObject)
				jr.Skip();
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
					
					ReadToObjectStart(jr);
					var pdic = parameters.ToDictionary(p => p.Name);
					var serializer = JsonSerializer.Create(setting);
					var adic = new Dictionary<string, object>();
					for (;;)
					{
						var prop = ReadPropertyName(jr);
						if (prop == null)
							break;
						ParameterInfo p;
						if (pdic.TryGetValue(prop, out p))
						{
							var st = p.ParameterType.IsInterfaceType() ? ServiceDetector.GetServiceType(p.ParameterType) : ServiceType.Unknown;
							if (st == ServiceType.Normal)
							{
								Skip(jr);
								continue;
							}
							if (st == ServiceType.Managed)
							{
								SkipComment(jr);
								sc.ServiceIdents.Add(prop, (string)jr.Value);
								continue;
							}
							SkipComment(jr);
							adic[prop] = serializer.Deserialize(jr, p.ParameterType);
						}
						else
						{
							Skip(jr);
						}
					}
					
					args = parameters.Select(p => { object a; adic.TryGetValue(p.Name, out a); return a; }).ToArray();
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
			string id;
			ServiceIdents.TryGetValue(Path, out id);
			return id;
		}
	}
}
