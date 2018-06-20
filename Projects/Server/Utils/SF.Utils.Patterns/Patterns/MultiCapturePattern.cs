using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SF.Utils.Patterns
{

	public class MultipleCapturePattern<TItem> : IPattern<TItem>
	{
		public bool Required { get; }
		public MultipleCapturePattern(bool Required)
		{
			this.Required = Required;
		}
		public MatchResult Evaluate(MatchInput<TItem> Input)
		{
			if (Input.Begin >= Input.End)
				return new MatchResult
				{
					Success = !Required,
					EvalEnd = Input.End
				};
			return new MatchResult
			{
				Matchs = new[] { new Matchs.MultipleItemsMatch<TItem>(Input.Path,Input.Items.Skip(Input.Begin).Take(Input.End-Input.Begin).ToArray()) },
				EvalEnd = Input.End,
				Success = true
			};
		}
	}
}
