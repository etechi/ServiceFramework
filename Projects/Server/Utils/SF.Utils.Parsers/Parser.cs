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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Utils.Parsers
{
	public delegate Result<T> Parser<T>(Content input);

	public class ParserBuildContext
	{
		internal Func<Parser<Expression>> RootExprReference { get; set; }
		internal Func<Parser<Expression>> PrimativeExprReference { get; set; }
		public Func<string, Expression> BuildVarExpression { get; set; }
		public Func<Expression,string,Expression> BuildMemberExpression { get; set; }
	}
	public static class Parser
	{
		
		public static Parser<Expression> Variable(ParserBuildContext Context)
		{
			var var = Lexer.Variable();
			return input =>
			{
				var re = var(input);
				if (re.Value.IsNone)
					return new Result<Expression>(null, input);

				return re.Left.WithValue(Context.BuildVarExpression(re.Value.ToString()));
			};
		}
		public static Parser<Expression> ParenthesesExpr(ParserBuildContext Context)
		{
			var lp = Lexer.Token("(");
			var rp = Lexer.Token(")");
			var er = Context.RootExprReference;
			return input =>
			{
				var l = lp(input);
				if (l.Value.IsNone)
					return Result.None<Expression>(input);
				var re = er()(l.Left);
				if (re.Value == null)
					throw new InvalidOperationException();
				var r = rp(re.Left);
				if(r.Value.IsNone)
					throw new InvalidOperationException();
				return r.Left.WithValue(re.Value);
			};
		}
		public static Parser<Expression> StringExpr(ParserBuildContext Context)
		{
			var se = Lexer.String('\"');
			return input =>
			{
				var re = se(input);
				if(re.Value.Value.IsEmpty)
					return Result.None<Expression>(input);
				return re.Left.WithValue(
					(Expression)Expression.Constant(
						Newtonsoft.Json.JsonConvert.DeserializeObject<string>(re.Value.Value.ToString())
						)
					);
			};
		}
		public static Parser<Expression> BoolExpr(ParserBuildContext Context)
		{
			var ttrue = Lexer.Token("true");
			var tfalse = Lexer.Token("false");
			return input =>
			{
				var l = ttrue(input);
				if (!l.Value.IsNone)
					return l.Left.WithValue((Expression)Expression.Constant(true));
				l = tfalse(input);
				if (!l.Value.IsNone)
					return l.Left.WithValue((Expression)Expression.Constant(false));
				return Result.None<Expression>(input);
			};
		}
		public static Parser<Expression> IntegerExpr(ParserBuildContext Context)
		{
			var e = Lexer.IntegerNumber();
			return input =>
			{
				var l = e(input);
				if (l.Value.IsNone)
					return Result.None<Expression>(input);
				var s = l.Value.Value.ToString();
				//if (s.Contains('.') || s.Contains('e') || s.Contains('E'))
				return l.Left.WithValue((Expression)Expression.Constant(int.Parse(s)));
				//else
				//	return l.Left.WithValue((Expression)Expression.Constant(int.Parse(s)));
			};
		}
		public static Parser<Expression> NumberExpr(ParserBuildContext Context)
		{
			var e = Lexer.RealNumber();
			return input =>
			{
				var l = e(input);
				if (l.Value.IsNone)
					return Result.None<Expression>(input);
				var s = l.Value.Value.ToString();
				//if (s.Contains('.') || s.Contains('e') || s.Contains('E'))
					return l.Left.WithValue((Expression)Expression.Constant(double.Parse(s)));
				//else
				//	return l.Left.WithValue((Expression)Expression.Constant(int.Parse(s)));
			};
		}
		public static Parser<Expression> MemberExpr(ParserBuildContext Context)
		{
			var ef = Context.PrimativeExprReference;
			var pt = Lexer.Token(".");
			var prop = Lexer.Variable();
			return input =>
			{
				var l = ef()(input);
				if (l.Value==null)
					return Result.None<Expression>(input);
				var ptre = pt(l.Left);
				if (ptre.Value.IsNone)
					return l;
				var pp = prop(ptre.Left);
				if (pp.Value.IsNone)
					throw new InvalidOperationException();
				return pp.Left.WithValue(
					Context.BuildMemberExpression(l.Value, pp.Value.Value.ToString())
					);
			};
		}
		public static Parser<Expression> PrimativeExpr(ParserBuildContext Context)
		{
			var es = new[]{
				ParenthesesExpr(Context),
				NumberExpr(Context),
				BoolExpr(Context),
				StringExpr(Context),
				MemberExpr(Context),
				Variable(Context),
			};
			Parser<Expression> exp = null;
			Context.PrimativeExprReference = () => exp;
			exp=input =>
			{
				return es.Select(e => e(input))
					.Where(e => e.Value != null)
					.FirstOrDefault();
			};
			return exp;
		}
		public static Parser<Expression> PrefixUnaryExpr(
			Parser<Expression> baseParser,
		    Func<Expression, string, Expression> BuildExpression,
			params string[] Operators
			)
		{
			var ops = Operators.Select(o => Lexer.Token(o)).ToArray();
			return input =>
			{
				var o = ops.Select(op => op(input)).Where(re => !re.Value.IsNone).FirstOrDefault();
				if (o.Value.IsNone)
					return baseParser(input);

				var right = baseParser(o.Left);
				if (right.Value == null)
					return Result.None<Expression>(input);
				return right.Left.WithValue(
					BuildExpression(right.Value, o.Value.Value.ToString())
					);
			};
		}
		public static Parser<Expression> PostUnaryExpr(
		   Parser<Expression> baseParser,
		   Func<Expression, string, Expression> BuildExpression,
		   params string[] Operators
		   )
		{
			var ops = Operators.Select(o => Lexer.Token(o)).ToArray();
			return input =>
			{
				var baseExpr = baseParser(input);
				if (baseExpr.Value == null)
					return Result.None<Expression>(input);

				var o = ops.Select(op => op(baseExpr.Left)).Where(re => !re.Value.IsNone).FirstOrDefault();
				if (o.Value.IsNone)
					return baseExpr;
				
				return o.Left.WithValue(
					BuildExpression(baseExpr.Value, o.Value.Value.ToString())
					);
			};
		}
		public static Parser<Expression> UnaryExpr(ParserBuildContext Context)
		{
			var ce = PrimativeExpr(Context);
			return PrefixUnaryExpr(
				ce,
				(e, o) =>
					o=="~"? Expression.Not(e):
					o=="-"? Expression.Negate(e):
					throw new InvalidOperationException(),
				"~",
				"-"
				);
		}
		public static Parser<Expression> BinaryExpr(
			Parser<Expression> leftParser,
			Parser<Expression> rightParser,
			Func<Expression,Expression,string,Expression> BuildExpression,
			params string[] Operators
			)
		{
			var ops = Operators.Select(o => Lexer.Token(o)).ToArray();
			return input =>
			{
				var left = leftParser(input);
				if (left.Value == null)
					return Result.None<Expression>(input);

				var o = ops.Select(op => op(left.Left)).Where(re => !re.Value.IsNone).FirstOrDefault();
				if (o.Value.IsNone)
					return left;

				var right = rightParser(o.Left);
				if (right.Value == null)
					return Result.None<Expression>(input);

				return right.Left.WithValue(
					BuildExpression(
						left.Value,
						right.Value,
						o.Value.Value.ToString()
						)
					);
			};
		}
		
		public static Parser<Expression> MulAndDivExpr(ParserBuildContext Context)
		{
			var ue = UnaryExpr(Context);
			return BinaryExpr(
				ue, 
				ue,
				(l,r,o)=>
					o=="*"?
					Expression.Multiply(l,r): 
					Expression.Divide(l, r),
				"*", 
				"/"
				);

		}
		public static Parser<Expression> AddAndSubExpr(ParserBuildContext Context)
		{
			var mde = MulAndDivExpr(Context);
			return BinaryExpr(
				mde,
				mde,
				(l, r, o) =>
					o == "+" ?
					Expression.Add(l, r) :
					Expression.Subtract(l, r),
				"*",
				"/"
				);
		}
		public static Parser<Expression> BitBinExpr(ParserBuildContext Context)
		{
			var mde = AddAndSubExpr(Context);
			return BinaryExpr(
				mde,
				mde,
				(l, r, o) =>
					o == "|" ?
					Expression.Or(l, r) :
					Expression.And(l, r),
				"|",
				"&"
				);
		}
		public static Parser<Expression> CompareExpr(ParserBuildContext Context)
		{
			var mde = BitBinExpr(Context);
			return BinaryExpr(
				mde,
				mde,
				(l, r, o) =>
					o == "==" ? Expression.Equal(l, r) :
					o == "!=" ? Expression.NotEqual(l, r) :
					o == ">" ? Expression.GreaterThan(l, r) :
					o == "<" ? Expression.LessThan(l, r) :
					o == ">=" ? Expression.GreaterThanOrEqual(l, r) :
					o == "<=" ? Expression.LessThanOrEqual(l, r) :
					throw new InvalidOperationException(),
				"==",
				"!=",
				">",
				"<",
				">=",
				"<="
				);
		}

		public static Parser<Expression> LogicNotExpr(ParserBuildContext Context)
		{
			var ce = CompareExpr(Context);
			return PrefixUnaryExpr(
				ce,
				(e, o) =>Expression.Equal(e,Expression.Constant(false)),
				"!"
				);
		}

		public static Parser<Expression> LogicBinExpr(ParserBuildContext Context)
		{
			var lne = LogicNotExpr(Context);
			return BinaryExpr(
				lne,
				lne,
				(l, r, o) => 
					o == "&&" ? Expression.AndAlso(l, r) :
					o == "and" ? Expression.AndAlso(l, r) :
					o == "||" ? Expression.OrElse(l, r) :
					o == "or" ? Expression.OrElse(l, r) :
					throw new InvalidOperationException(),
				"&&",
				"and",
				"||" ,
				"or"				
				);
		}

		public static Parser<Expression> Expr(ParserBuildContext Context)
		{
			Parser<Expression> p = null;
			Context.RootExprReference = () => p;

			p=LogicBinExpr(Context);
			return p;
		}
	}
}

