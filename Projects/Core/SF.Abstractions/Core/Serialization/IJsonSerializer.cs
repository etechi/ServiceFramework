using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Core.Serialization
{
	public class JsonSetting
	{
		public bool WithType { get; set; }
		public bool IgnoreDefaultValue { get; set; }
	}
	public interface IJsonSerializer
	{
		string Serialize(object Object, Type Type, JsonSetting Setting = null);
		object Deserialize(string Text, Type Type, JsonSetting Setting = null);
	}
	
}
