using System;
using System.Collections.Generic;
using System.Text;

namespace SF
{
	public static class Json
	{
		public static Serialization.IJsonSerializer DefaultSerializer { get; set; } = Serialization.Newtonsoft.JsonSerializer.Instance;
		public static Serialization.JsonSetting DefaultSetting { get; set; } = new Serialization.JsonSetting
		{
			IgnoreDefaultValue = true,
			WithType = false
		};
		public static string Stringify<T>(T obj)
		{
			return DefaultSerializer.Serialize(obj, typeof(T), DefaultSetting);
		}
		public static T Parse<T>(string str)
		{
			return (T)DefaultSerializer.Deserialize(str, typeof(T), DefaultSetting);
		}
	}
}
