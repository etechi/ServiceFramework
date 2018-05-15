#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using System;

namespace SF.Utils.Parsers
{
	public static class Lexer
	{
		public static unsafe Parser<Token> Spaces(Func<char,bool> IsSpace=null)
		{
			if (IsSpace == null)
				IsSpace = char.IsWhiteSpace;
			return input =>
			{
				if (input.IsEmpty)
					return new Result<Token>(
						Parsers.Token.None,
						input
						);
				using (var pin = input.Pin())
				{
					var p = (char*)pin.Pointer;
					var b = p;
					var i = b;
					var e = p + input.Length;
					for (;i<e;i++)
						if (!IsSpace(*i))
							break;
					var l = (int)(i - b);
					if (l == 0)
						return new Result<Token>(Parsers.Token.None, input);
					return new Result<Token>(
						new Token("#space", input.Cut(l)),
						input.Skip(l)
						);
				}
			};
		}
		//public static Parser<Token> Tokens(params string[] tokens)
		//{
		//	var tokenDict=(from t in tokens
		//	 let fc = t[0]
		//	 group t by fc into g
		//	 select g
		//	).ToDictionary(
		//		g => g.Key,
		//		g => g.GroupBy(t => t.Length).OrderByDescending(gi => gi.First().Length).Select(gi => gi.Distinct().ToArray()).ToArray()
		//		);


		//	return input =>
		//	{
		//		if(!tokenDict.TryGetValue(input.FirstChar,out var ts))
		//			return new Result<Token>(
		//				Token.None,
		//				input
		//				);
		//		foreach(var tg in ts)
		//		{
		//			if (tg[0].Length > input.Length)
		//				continue;
		//			foreach(var t in tg)
		//				input.s
		//		}
		//	};
		//}

		public unsafe static Parser<Token> Variable()
		{
			return input =>
			{
				if (input.IsEmpty)
					return new Result<Token>(
						Parsers.Token.None,
						input
						);
				if (!char.IsLetter(input.FirstChar))
					return new Result<Token>(Parsers.Token.None, input);

				using(var pin = input.Pin())
				{
					char* p = (char*)pin.Pointer;
					var b = p;
					var i = b+1;
					var e = p + input.Length;
					for (; i < e; i++)
						if (!char.IsLetterOrDigit(*i))
							break;
					var l = (int)(i - b);
					return new Result<Token>(
						new Token("#var", input.Cut(l)),
						input.Skip(l)
						);
				}
			};
		}

		public static Parser<Token> Token(string token)
		{
			var tok = token.ToCharArray();
			return input =>
			{
				return input.Span.StartsWith(tok) ?
					new Result<Token>(
						new Token(token, input.Cut(token.Length)),
						input.Skip(token.Length)
						) :
					new Result<Token>(Parsers.Token.None, input);
			};
		}

		enum NumberState
		{
			Sign,
			Integer,
			Decimal,
			ExpSign,
			ExpInteger,
			ExpDecimal
		}
		public static unsafe Parser<Token> IntegerNumber()
		{
			return input =>
			{
				if (input.IsEmpty)
					return new Result<Token>(
						Parsers.Token.None,
						input
						);
				using (var pin=input.Pin())
				{
					char* p = (char*)pin.Pointer;
					var b = p ;
					var i = b;
					var e = p + input.Length;
					var s = NumberState.Sign;
					var end = false;
					for (; i < e; i++)
					{
						switch (*i)
						{
							case '-':
							case '+':
								if (s == NumberState.Sign)
									s = NumberState.Integer;
								else
									end = true;
								break;
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								switch (s)
								{
									case NumberState.Sign:
										s = NumberState.Integer;
										break;
									case NumberState.Integer:
										break;
									default:
										end = true;
										break;
								}
								break;
							default:
								end = true;
								break;
						}
						if (end)
							break;
					}
					var l = (int)(i - b);
					if (l == 0)
						return new Result<Token>(Parsers.Token.None, input);
					return new Result<Token>(
						new Token("#int", input.Cut(l)),
						input.Skip(l)
						);
				}
			};
		}
		public static unsafe Parser<Token> RealNumber(){
			return input =>
			{
				if (input.IsEmpty)
					return new Result<Token>(
						Parsers.Token.None,
						input
						);
				using (var pin = input.Pin())
				{
					var p = (char*)pin.Pointer;
					var b = p;
					var i = b;
					var e = p + input.Length;
					var s = NumberState.Sign;
					var end = false;
					for (; i < e; i++)
					{
						switch (*i)
						{
							case '-':
							case '+':
								if (s == NumberState.Sign)
									s = NumberState.Integer;
								else if (s == NumberState.ExpSign)
									s = NumberState.ExpInteger;
								else
									end = true;
								break;
							case 'e':
							case 'E':
								if (s == NumberState.Integer || s == NumberState.Decimal)
									s = NumberState.ExpSign;
								else
									end = true;
								break;
							case '.':
								if (s == NumberState.Sign || s == NumberState.Integer)
									s = NumberState.Decimal;
								else if (s == NumberState.ExpSign || s == NumberState.ExpInteger)
									s = NumberState.ExpDecimal;
								else
									end = true;
								break;
							case '0':
							case '1':
							case '2':
							case '3':
							case '4':
							case '5':
							case '6':
							case '7':
							case '8':
							case '9':
								switch (s)
								{
									case NumberState.Sign:
										s = NumberState.Integer;
										break;
									case NumberState.ExpSign:
										s = NumberState.ExpInteger;
										break;
									case NumberState.Integer:
									case NumberState.Decimal:
									case NumberState.ExpInteger:
									case NumberState.ExpDecimal:
										break;
									default:
										end = true;
										break;
								}
								break;
							default:
								end = true;
								break;
						}
						if (end)
							break;
					}
					var l = (int)(i - b);
					if (l == 0)
						return new Result<Token>(Parsers.Token.None, input);
					return new Result<Token>(
						new Token("#real", input.Cut(l)),
						input.Skip(l)
						);
				}
			};
		}

		public static unsafe Parser<Token> String(char Quot)
		{
			return input =>
			{
				if (input.IsEmpty)
					return new Result<Token>(
						Parsers.Token.None,
						input
						);
				if(input.FirstChar!=Quot)
					return new Result<Token>(Parsers.Token.None, input);

				var osp = input.Skip(1).Span;
				var sp = osp;
				var off = 0;
				int i;
				for (; ; )
				{
					i = sp.IndexOf(Quot);
					if (i == -1)
						throw new ArgumentException();
					i += off;
					if (i == 0 || osp[i - 1] != '\\')
						break;
					off = i + 1;
					sp = osp.Slice(off);
				}
				var l = i + 1;
				return new Result<Token>(
					new Token("#string", input.Cut(l)),
					input.Skip(l)
					);
			};
		}
	}
}

