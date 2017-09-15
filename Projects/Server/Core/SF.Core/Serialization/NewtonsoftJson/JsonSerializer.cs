using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq.Expressions;
using System.Reflection;
using SF.Reflection;
namespace SF.Serialization.Newtonsoft
{
	
	public class JsonSerializer : IJsonSerializer
	{
		public static IJsonSerializer Instance { get; } = new JsonSerializer();

		JsonSerializerSettings MapSetting(JsonSetting Setting)
		{
			return new JsonSerializerSettings
			{
				ContractResolver =  FixedContractResolver.Instance,
				//DateFormatString = "yyyy-MM-ddTHH:mm:dd",
				DefaultValueHandling = Setting?.IgnoreDefaultValue ??true ? DefaultValueHandling.IgnoreAndPopulate:DefaultValueHandling.Populate,
				DateParseHandling = global::Newtonsoft.Json.DateParseHandling.DateTime,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = Setting?.WithType ?? false ? TypeNameHandling.Objects:TypeNameHandling.None,
				Converters = new[] {
					(JsonConverter) new global::Newtonsoft.Json.Converters.StringEnumConverter(),
					(JsonConverter)new ExpandoObjectConverter()
				},
				ReferenceLoopHandling = global::Newtonsoft.Json.ReferenceLoopHandling.Ignore,
			};
		}

		public object Deserialize(string Text, Type Type, JsonSetting Setting)
		{
			return JsonConvert.DeserializeObject(Text, Type, MapSetting(Setting));
		}

		public string Serialize(object Object, Type Type, JsonSetting Setting)
		{
			return JsonConvert.SerializeObject(Object,Type,MapSetting(Setting));
		}
	}
}
