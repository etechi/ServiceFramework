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

namespace SF.Sys
{
	public struct Option<T>
	{
		public T Value { get; set; }
		public bool HasValue { get; set; }

		public static implicit operator Option<T>(T value)
		{
			return new Option<T>
			{
				Value = value,
				HasValue = true
			};
		}
	}
	public static class OptionExtension
	{
		public static T ValueOrDefault<T>(this Option<T> value)
		{
			return value.HasValue ? value.Value : default(T);
		}
	}

}
