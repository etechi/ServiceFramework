using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq.Expressions;
using System.Reflection;
using SF.Reflection;
namespace SF.Serialization.Newtonsoft
{
	class FixedContractResolver : DefaultContractResolver
	{
		private FixedContractResolver() { }
		public static FixedContractResolver Instance { get; } = new FixedContractResolver();
		protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
		{
			var p = base.CreateProperty(member, memberSerialization);
			if ((member is PropertyInfo pi) && pi.PropertyType.IsEnumType())
				p.DefaultValue = Enum.ToObject(pi.PropertyType, -1);
			return p;
		}
	}
	public class JsonSerilaizer : IJsonSerializer
	{
		public static JsonSerializer Instance { get; } = new JsonSerializer();

		JsonSerializerSettings MapSetting(JsonSetting Setting)
		{
			return new JsonSerializerSettings
			{
				ContractResolver =  FixedContractResolver.Instance,
				//DateFormatString = "yyyy-MM-ddTHH:mm:dd",
				DefaultValueHandling = Setting.IgnoreDefaultValue? DefaultValueHandling.IgnoreAndPopulate:DefaultValueHandling.Populate,
				DateParseHandling = global::Newtonsoft.Json.DateParseHandling.DateTime,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				MissingMemberHandling = MissingMemberHandling.Ignore,
				NullValueHandling = NullValueHandling.Ignore,
				TypeNameHandling = Setting.WithType? TypeNameHandling.Objects:TypeNameHandling.None,
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
