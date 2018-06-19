using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns.Matchs
{

	public class MultipleItemsMatch<TItem> : IMultipleItemsMatch<TItem>
	{
		public MultipleItemsMatch(string Path, TItem[] items)
		{
			this.Items = items;
			this.Path = Path;
		}
		public IEnumerable<TItem> Items { get; }

		public string Path { get; }
	}

}
