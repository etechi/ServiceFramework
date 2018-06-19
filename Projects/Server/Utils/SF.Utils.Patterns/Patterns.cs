using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public struct MatchResult
	{
		public bool Success { get; set; }
		public int EvalEnd { get; set; }
		public IEnumerable<IMatch> Matchs { get; set; }
	}
	public struct MatchInput<TItem>
	{
		public TItem[] Items { get; set; }
		public string Path { get; set; }
		public int Begin { get; set; }
		public int End { get; set; }
	}
	public interface IPattern<TItem>
	{
		MatchResult Evaluate(MatchInput<TItem> Input);
	}
}
