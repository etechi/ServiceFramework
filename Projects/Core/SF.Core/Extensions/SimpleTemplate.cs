using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF
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
