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


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using SF.Sys.Reflection;

namespace SF.Sys.Serialization.Newtonsoft
{
	public class TimeSpanConverter : JsonConverter
	{
		/// <summary>
		/// Gets or sets a value indicating whether the written enum text should be camel case.
		/// </summary>
		/// <value><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</value>
		public bool CamelCaseText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets a value indicating whether integer values are allowed when deserializing.
		/// </summary>
		/// <value><c>true</c> if integers are allowed when deserializing; otherwise, <c>false</c>.</value>
		public bool AllowIntegerValues
		{
			get;
			set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Converters.StringEnumConverter" /> class.
		/// </summary>
		public TimeSpanConverter()
		{
			this.AllowIntegerValues = true;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Newtonsoft.Json.Converters.StringEnumConverter" /> class.
		/// </summary>
		/// <param name="camelCaseText"><c>true</c> if the written enum text will be camel case; otherwise, <c>false</c>.</param>
		public TimeSpanConverter(bool camelCaseText) : this()
		{
			this.CamelCaseText = camelCaseText;
		}

		/// <summary>
		/// Writes the JSON representation of the object.
		/// </summary>
		/// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
		/// <param name="value">The value.</param>
		/// <param name="serializer">The calling serializer.</param>
		public override void WriteJson(JsonWriter writer, object value, global::Newtonsoft.Json.JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			else if(value is  TimeSpan? )
			{
				var t = (TimeSpan?)value;
				if (t.HasValue)
					writer.WriteValue(t.Value.ToString());
				else
					writer.WriteNull();
			}
			else
				writer.WriteValue(((TimeSpan)value).ToString());
		}

		/// <summary>
		/// Reads the JSON representation of the object.
		/// </summary>
		/// <param name="reader">The <see cref="T:Newtonsoft.Json.JsonReader" /> to read from.</param>
		/// <param name="objectType">Type of the object.</param>
		/// <param name="existingValue">The existing value of object being read.</param>
		/// <param name="serializer">The calling serializer.</param>
		/// <returns>The object value.</returns>
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, global::Newtonsoft.Json.JsonSerializer serializer)
		{
			if (reader.TokenType != JsonToken.Null)
			{
				var result = reader.TokenType == JsonToken.String ? TimeSpan.Parse(reader.Value.ToString()) :
					reader.TokenType == JsonToken.Float ||
					reader.TokenType == JsonToken.Integer ? TimeSpan.FromDays(Convert.ToDouble(reader.Value)):
				if (objectType == typeof(TimeSpan?))
					return (TimeSpan?)result;
				else
					return result;
			}
			else if(objectType==typeof(TimeSpan?))
			{
				return (TimeSpan?)null;
			}
			return TimeSpan.MinValue;
		}

		/// <summary>
		/// Determines whether this instance can convert the specified object type.
		/// </summary>
		/// <param name="objectType">Type of the object.</param>
		/// <returns>
		/// <c>true</c> if this instance can convert the specified object type; otherwise, <c>false</c>.
		/// </returns>
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(TimeSpan) || objectType == typeof(TimeSpan?);
		}
	}
}
