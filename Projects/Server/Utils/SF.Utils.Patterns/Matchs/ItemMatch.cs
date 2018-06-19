using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns.Matchs
{

	public class ItemMatch<TItem> : IItemMatch<TItem>
	{
		public ItemMatch(string Path,TItem item)
		{
			this.Path = Path;
			this.Item= item;
		}
		public TItem Item { get; }

		public string Path { get; }
	}

}
