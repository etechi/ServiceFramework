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

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SF.Sys.Reflection;
using System;
using System.Reflection;
namespace SF.Sys.Serialization.Newtonsoft
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
		public static JsonSerializerSettings ApplySetting(JsonSerializerSettings JsonSerializerSettings, JsonSetting Setting)
		{
			JsonSerializerSettings.ContractResolver = FixedContractResolver.Instance;
			//DateFormatString = "yyyy-MM-ddTHH:mm:dd",
			JsonSerializerSettings.DefaultValueHandling = Setting?.IgnoreDefaultValue ?? true ? DefaultValueHandling.IgnoreAndPopulate : DefaultValueHandling.Populate;
			JsonSerializerSettings.DateParseHandling = global::Newtonsoft.Json.DateParseHandling.DateTime;
			JsonSerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind;
			JsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
			JsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
			JsonSerializerSettings.TypeNameHandling = Setting?.WithType ?? false ? TypeNameHandling.Objects : TypeNameHandling.None;
			JsonSerializerSettings.Converters = new[] {
				(JsonConverter) new global::Newtonsoft.Json.Converters.StringEnumConverter(),
				//(JsonConverter) new TimeSpanConverter(),
				(JsonConverter)new ExpandoObjectConverter(),
				new OptionConverter()
			};
			JsonSerializerSettings.ReferenceLoopHandling = global::Newtonsoft.Json.ReferenceLoopHandling.Ignore;

			return JsonSerializerSettings;
		}
		public static JsonSerializerSettings MapSetting(JsonSetting Setting)
		{
			return ApplySetting(new JsonSerializerSettings(), Setting);
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
