﻿#region Apache License Version 2.0
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

using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Sys
{
	public static class Json
	{
		public static Serialization.IJsonSerializer DefaultSerializer { get; set; }
			= new SF.Sys.Serialization.Newtonsoft.JsonSerializer();
		public static Serialization.JsonSetting DefaultSetting { get; set; } = new Serialization.JsonSetting
		{
			//IgnoreDefaultValue = true,
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
