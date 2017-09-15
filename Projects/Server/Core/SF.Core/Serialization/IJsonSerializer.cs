using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Serialization
{
	public class JsonSetting
	{
		public bool WithType { get; set; }
		public bool IgnoreDefaultValue { get; set; }
	}
	public interface IJsonSerializer
	{
		string Serialize(object Object, Type Type, JsonSetting Config=null);
		object Deserialize(string Text, Type Type, JsonSetting Config=null);
	}
	
}
