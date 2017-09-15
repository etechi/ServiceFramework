using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Serialization
{
	
	public static class JsonSerializerExtension
	{
		public static string Serialize<T>(this IJsonSerializer Serializer,T obj,JsonSetting Setting=null)
		{
			return Serializer.Serialize(obj, typeof(T), Setting);
		}
		public static T Deserialize<T>(this IJsonSerializer Serializer, string Text, JsonSetting Config = null)
		{
			return (T)Serializer.Deserialize(Text, typeof(T), Config);
		}
	}
	
}
