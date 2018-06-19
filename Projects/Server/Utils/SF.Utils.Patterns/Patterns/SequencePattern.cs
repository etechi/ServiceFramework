using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns
{
	public struct PatternSetting<TItem>
	{
		public IPattern<TItem> Pattern { get; set; }
		public bool Required { get; set; }
	}
	public class SequencePattern<TItem> : IPattern<TItem>
	{
		PatternSetting<TItem>[] _children;
		public SequencePattern(PatternSetting<TItem>[] patterns)
		{
			_children = patterns;
		}

		public MatchResult Evaluate(MatchInput<TItem> Input)
		{
		
			var ci = 0;
			var pos = Input.Begin;
			List<IMatch> Matchs = null;
			while (pos < Input.End && ci<_children.Length)
			{
				var c = _children[ci];
				var re = c.Pattern.Evaluate(new MatchInput<TItem> {
					Items = Input.Items,
					Begin = pos,
					End = Input.End,
					Path=Input.Path
					}
				);
				if (re.Success)
				{
					pos = re.EvalEnd;
					ci++;

					if (re.Matchs?.Any() ?? false)
					{
						if (Matchs == null) Matchs = new List<IMatch>();
						Matchs.AddRange(re.Matchs);
					}
					continue;
				}
				if (!c.Required)
				{
					ci++;
					continue;
				}
				return new MatchResult();
			}
			
			while (ci < _children.Length)
			{
				if (_children[ci].Required)
					return new MatchResult();
				ci++;
			}
			return new MatchResult
			{
				Success = true,
				Matchs = Matchs,
				EvalEnd = pos
			};
		}
		
	}
	
}
