using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SF.Utils.Patterns
{
	public class SingleCapturePattern<TItem> : IPattern<TItem>
	{
		public MatchResult Evaluate(MatchInput<TItem> Input)
		{
			if (Input.Begin >= Input.End)
				return default;
			return new MatchResult
			{
				Matchs = new[] { new Matchs.ItemMatch<TItem>(Input.Path, Input.Items[Input.Begin]) },
				EvalEnd = Input.Begin + 1,
				Success = true
			};
		}
	}
}
