using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Reflection;
namespace SF.Core.Serialization.Newtonsoft
{
	class OptionConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsGeneric() && objectType.GetGenericTypeDefinition() == typeof(Option<>);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
		{
			return Activator.CreateInstance(objectType,serializer.Deserialize(reader,objectType.GetGenericArguments()[0]));
		}

		public override void WriteJson(JsonWriter writer, object value, global::Newtonsoft.Json.JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}
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
					(JsonConverter)new ExpandoObjectConverter(),
					new OptionConverter()
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
