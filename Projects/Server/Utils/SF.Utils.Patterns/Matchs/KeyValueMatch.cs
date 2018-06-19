using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public class KeyValueMatch : IKeyValueMatch
	{
		public string Key { get; }

		public string Value { get; }

		public string Path { get; }

		public KeyValueMatch(string Path,string Key,string Value)
		{
			this.Path = Path;
			this.Key = Key;
			this.Value = Value;

		}
	}

}
