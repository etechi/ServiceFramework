using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public interface IMatch
	{
		string Path { get; }
	}
	public interface IItemMatch<TItem> : IMatch
	{
		TItem Item { get; }
	}
	public interface IMultipleItemsMatch<TItem> : IMatch
	{
		IEnumerable<TItem> Items { get; }
	}
	public interface IKeyValueMatch : IMatch
	{
		string Key { get; }
		string Value { get; }
	}
	public interface ITextMatch : IMatch
	{
		string Text { get; }
	}
	
}
