using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public class TextPattern<TItem> : IPattern<TItem>
	{
		Func<TItem, string> FuncGetItemText { get; }
		string Value { get; }
		public TextPattern(string value,Func<TItem,string> FuncGetItemText)
		{
			this.Value = value;
			this.FuncGetItemText = FuncGetItemText;
		}
	

		public MatchResult Evaluate(MatchInput<TItem> Input)
		{
			if (Input.Begin >= Input.End)
				return new MatchResult { };
			if (FuncGetItemText(Input.Items[Input.Begin]) == Value)
				return new MatchResult { EvalEnd = Input.Begin + 1, Success = true };
			return new MatchResult { };
		}
	}
}
