using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
namespace SF.Utils.Patterns.Matchs
{

	public class TextMatch : ITextMatch
	{
		public TextMatch(string Path,string Text)
		{
			this.Path = Path;
			this.Text = Text;
		}
		public string Text { get; }

		public string Path { get; }
	}

}
