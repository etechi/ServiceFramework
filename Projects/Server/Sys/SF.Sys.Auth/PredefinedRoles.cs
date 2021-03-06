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

using System.Collections.Generic;
using System.Linq;

namespace SF.Sys.Auth
{
	public class PredefinedRoles
	{
		public const string 系统管理员 = nameof(系统管理员);
		public const string 安全专员 = nameof(安全专员);
		public const string 运营专员 = nameof(运营专员);
		public const string 媒介专员= nameof(媒介专员);
		public const string 客服专员 = nameof(客服专员);
		public const string 财务专员 = nameof(财务专员);
		public const string 销售专员= nameof(销售专员);

		public static IReadOnlyList<string> Items { get; } =
			typeof(PredefinedRoles).
			GetFields(
				System.Reflection.BindingFlags.Static |
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.GetField
				)
			.Select(f=> (string)f.GetValue(null)).ToArray()
			;
	}
}
