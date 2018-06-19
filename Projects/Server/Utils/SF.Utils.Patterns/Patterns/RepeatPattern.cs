using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public class RepeatPattern<TItem> : IPattern<TItem>
	{
		IPattern<TItem> Child { get; }
		bool MatchAllItems { get; }
		public RepeatPattern(IPattern<TItem> child, bool MatchAllItems)
		{
			this.Child = child;
			this.MatchAllItems = MatchAllItems;
		}

		public MatchResult Evaluate(MatchInput<TItem> Input)
		{
			var pos = Input.Begin;
			List<IMatch> Matchs = null;
			while(pos<Input.End)
			{
				var re = Child.Evaluate(new MatchInput<TItem>
				{
					Begin = pos,
					Path=Input.Path,
					End = Input.End,
					Items = Input.Items
				});
				if (re.Success)
				{
					if (re.Matchs?.Any() ?? false)
					{
						if (Matchs == null)
							Matchs = new List<IMatch>();
						Matchs.AddRange(re.Matchs);
					}
					pos = re.EvalEnd;
					continue;
				}
				break;
			}
			if (pos == Input.End || !MatchAllItems)
				return new MatchResult
				{
					EvalEnd = pos,
					Matchs = Matchs,
					Success = true
				};
			return new MatchResult();
		}
	}
}
