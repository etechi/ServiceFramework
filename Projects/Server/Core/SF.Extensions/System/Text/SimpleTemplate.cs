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
using System.Text;
using System.Threading.Tasks;

namespace System.Text
{
	public static class SimpleTemplate
	{
	
		static readonly System.Text.RegularExpressions.Regex _reg_replace =
			new System.Text.RegularExpressions.Regex("\\{[^\\}]+\\}");

		public static string Eval(string tmpl, IDictionary<string, string> args)
		{
			if (tmpl == null)
				return string.Empty;
			return _reg_replace.Replace(tmpl, match =>
			{
				var key = match.Groups[0].Value;
				key = key.Substring(1, key.Length - 2).Trim();

				var i = key.IndexOf(':');
				var default_value = i == -1 ? null : key.Substring(i + 1);
				key = i == -1 ? key : key.Substring(0, i);
				string re;
				if (args.TryGetValue(key, out re))
					return re;
				if(default_value!=null)
					return default_value;
				return match.Groups[0].Value;
			});
		}
	}
}
