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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection;
namespace System.Linq.Expressions
{
	public static class ExpressionExtension
	{
		public static Expression Create(this Type type, params Expression[] args)
		{
			return Create(type, (IEnumerable<Expression>)args);
		}
		public static Expression Create(this Type type, IEnumerable<Expression> args)
		{
			return Expression.New(
				type.GetConstructor((from a in args select a.Type).ToArray()),
				args
				);
		}
		public static Expression ThrowException(this Expression expr)
		{
			return Expression.Throw(expr);
		}
		public static Expression Block(params Expression[] es)
		{
			return Expression.Block(es);
		}
		public static Expression Block(IEnumerable<Expression> es)
		{
			return Expression.Block(es);
		}
		public static Expression Assign(this Expression left, Expression right)
		{
			return Expression.Assign(left, right);
		}
		public static Expression Assign<T>(this Expression left, T right)
		{
			return Expression.Assign(left, Expression.Constant(right));
		}
		public static Expression WithNullDefault(this Expression expr,Func<Expression,Expression> Mapper=null, Expression defaultValue=null)
		{
			if (expr.Type.IsValue())
				return expr;

			if (expr.NodeType == ExpressionType.Parameter)
			{
				var result = Mapper == null ? expr : Mapper(expr);
				if (defaultValue != null && defaultValue.Type != result.Type)
					throw new InvalidOperationException($"空值默认值类型{defaultValue.Type}和表达式类型{result.Type}不同");
				return
					Expression.Condition(
						expr.Equal(Expression.Constant(null, expr.Type)),
						defaultValue ?? Expression.Constant(null, result.Type),
						result
						);
			}
			else
			{
				var v = Expression.Variable(expr.Type);
				var result = Mapper == null ? v : Mapper(v);
				if (defaultValue != null && defaultValue.Type != result.Type)
					throw new InvalidOperationException($"空值默认值类型{defaultValue.Type}和表达式类型{result.Type}不同");
				return Expression.Block(
					new[] { v },
					v.Assign(expr),
					Expression.Condition(
						v.Equal(Expression.Constant(null, expr.Type)),
						defaultValue ?? Expression.Constant(null, result.Type),
						result
						)
					);
			}
		}
		public static Expression CallMethod(this Expression obj, MethodInfo method, params Expression[] args)
		{
			return Expression.Call(
				obj,
				method,
				args
				);
		}
		public static Expression CallMethod(this Expression obj, string name, params Expression[] args)
		{
			return CallMethod(obj, obj.Type.FindMethod(name, BindingFlags.Public | BindingFlags.Instance), args);
		}
		public static Expression CallMethod(this Expression obj, string name, IEnumerable<Expression> args)
		{
			return CallMethod(obj, obj.Type.FindMethod(name, BindingFlags.Public | BindingFlags.Instance), args.ToArray());
		}
		public static MethodInfo FindMethod(this Type type, string name, BindingFlags flags)
		{
			var m = type.GetMethod(name, flags);
			if (m != null)
				return m;
			foreach (var t in type.GetInterfaces())
			{
				m = t.FindMethod(name, flags);
				if (m != null)
					return m;
			}
			return null;

			//type.method(name, BindingFlags.NonPublic | BindingFlags.Instance) ??
			//type.method(name, BindingFlags.NonPublic | BindingFlags.Static);
		}
		public static MethodInfo Method(this Type type, string name, System.Reflection.BindingFlags flags)
		{
			return type.GetMethod(name, flags);
		}
		public static MethodInfo Method(this Type type, string name, params Type[] types)
		{
			name += "<" + new string(',', types.Length - 1) + ">";
			var m =
				type.GetMethod(name, BindingFlags.Public | BindingFlags.Instance) ??
				type.GetMethod(name, BindingFlags.Public | BindingFlags.Static);

			//type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance) ??
			//type.GetMethod(name, BindingFlags.NonPublic | BindingFlags.Static);
			if (m == null)
				return null;
			return m.MakeGenericMethod(types);
		}
		public static MemberInfo Member(this Type type, string name)
		{
			return
				type.Property(name, BindingFlags.Public | BindingFlags.Instance) ??
				type.Property(name, BindingFlags.Public | BindingFlags.Static) ??
				type.Field(name, BindingFlags.Public | BindingFlags.Instance) ??
				type.Field(name, BindingFlags.Public | BindingFlags.Static);
			/*type.prop(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
			type.prop(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
			type.field(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField) ??
			type.field(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField);
			 */
		}
		public static MemberInfo Property(this Type type, string name, BindingFlags flags)
		{
			return type.GetProperties(flags).FirstOrDefault(p => p.Name == name);
		}
		public static MemberInfo Field(this Type type, string name, BindingFlags flags)
		{
			return type.GetField(name, flags);
		}

		public static Expression Loop(Expression body, LabelTarget break_target = null, LabelTarget continue_target = null)
		{
			return Expression.Loop(body, break_target, continue_target);
		}
		public static Expression Then(this Expression expr, params Expression[] true_expr)
		{
			return true_expr.Length == 1 ?
				Expression.IfThen(expr, true_expr[0]) :
				Expression.IfThen(expr, Block(true_expr));
		}
		public static Expression Then(this Expression expr, IEnumerable<Expression> true_expr)
		{
			return Then(expr, true_expr.ToArray());
		}
		public static Expression Then(this Expression expr, Expression true_expr, Expression false_expr)
		{
			return Expression.IfThenElse(expr, true_expr, false_expr);
		}
		public static Expression ThenLoop(this Expression cond, params Expression[] body)
		{
			var bl = Expression.Label();
			var b = body.Length == 1 ? body[0] : Block(body);
			return Loop(cond.Then(b, Expression.Break(bl)), bl);
		}

		public static Expression To(this Expression v, Type type)
		{
			return Expression.Convert(v, type);
		}
		public static Expression To<T>(this Expression v)
		{
			return v.To(typeof(T));
		}
		public static Expression UseEnumerator<T>(this Expression e, Func<Expression, Expression> body)
		{
			return e.To<IEnumerable<T>>().CallMethod("GetEnumerator").Use(body);
		}
		public static Type MakeGenericType(this Type type, IEnumerable<Type> types)
		{
			return type.MakeGenericType(types.ToArray());
		}
		public static Type MakeGenericType<T>(this Type type)
		{
			return type.MakeGenericType(typeof(T));
		}
		public static Type MakeGenericType<T1, T2>(this Type type)
		{
			return type.MakeGenericType(typeof(T1), typeof(T2));
		}
		public static Type MakeGenericType<T1, T2, T3>(this Type type)
		{
			return type.MakeGenericType(typeof(T1), typeof(T2), typeof(T3));
		}
		public static Type MakeGenericType<T1, T2, T3, T4>(this Type type)
		{
			return type.MakeGenericType(typeof(T1), typeof(T2), typeof(T3), typeof(T4));
		}
		public static Expression UseEnumerator(this Expression e, Type type, Func<Expression, Expression> body)
		{
			return e.To(typeof(IEnumerable<>).MakeGenericType(type)).CallMethod("GetEnumerator").Use(body);
		}
		public static Expression Iterator(this Expression e, Type type, Func<Expression, Expression> body)
		{
			if (e.Type != typeof(IEnumerator<>).MakeGenericType(type))
				throw new ArgumentException();
			var v = type.AsVariable();
			return e.CallMethod("MoveNext").ThenLoop(
				Expression.Block(
					new ParameterExpression[] { (ParameterExpression)v },
					v.Assign(e.GetMember("Current")),
					body(v)
				));
		}
		public static Expression Iterator(this Expression e, ParameterExpression arg, Func<Expression, Expression> body)
		{
			if (e.Type != typeof(IEnumerator<>).MakeGenericType(arg.Type))
				throw new ArgumentException();
			return e.CallMethod("MoveNext").ThenLoop(
				Expression.Block(
					arg.Assign(e.GetMember("Current")),
					body(arg)
				));
		}
		public static Expression Iterator<T>(this Expression e, Func<Expression, Expression> body)
		{
			return Iterator(e, typeof(T), body);
		}
		public static Expression ForEach<T>(this Expression e, Func<Expression, Expression> body)
		{
			return e.UseEnumerator<T>(ie => ie.Iterator<T>(body));
		}
		public static Expression ForEach(this Expression e, Type type, Func<Expression, Expression> body)
		{
			return e.UseEnumerator(type, ie => ie.Iterator(type, body));
		}
		public static Expression ForEach(this Expression e, ParameterExpression arg, Func<Expression, Expression> body)
		{
			return e.UseEnumerator(arg.Type, ie => ie.Iterator(arg, body));
		}
		public static Expression Use(this Expression e, Func<Expression, Expression> body)
		{
			var v = e.Type.AsVariable();
			return Expression.Block(
				new ParameterExpression[] { (ParameterExpression)v },
				new Expression[]{
					v.Assign(e),
					Expression.TryFinally(
						body(v),
						v.To<IDisposable>().CallMethod("Dispose")
						)
					}
				);
		}
		public static Expression While(this Expression body, Expression cond)
		{
			var bl = Expression.Label();
			return Loop(
				Block(
					body,
					Not(cond).Then(Expression.Break(bl))
				),
				bl);
		}
		public static Expression Call(this MethodInfo method, params Expression[] args)
		{
			return Expression.Call(
				null,
				method,
				args
				);
		}
		public static Expression Call(this Expression instance, MethodInfo method, params Expression[] args)
		{
			return Expression.Call(
				instance,
				method,
				args
				);
		}
		public static Expression AsParameter(this Type type, string name)
		{
			return Expression.Parameter(type, name);
		}

		public static Expression AsVariable(this Type type, string name = null)
		{
			return Expression.Variable(type, name);
		}
		public static Expression AsVariable<T>(string name = null)
		{
			return AsVariable(typeof(T), name);
		}

		public static Expression Add(this Expression left, Expression right)
		{
			return Expression.Add(left, right);
		}
		public static Expression Subtract(this Expression left, Expression right)
		{
			return Expression.Subtract(left, right);
		}
		public static Expression Multiply(this Expression left, Expression right)
		{
			return Expression.Multiply(left, right);
		}
		public static Expression Divide(this Expression left, Expression right)
		{
			return Expression.Divide(left, right);
		}
		public static Expression Modulo(this Expression left, Expression right)
		{
			return Expression.Modulo(left, right);
		}
		public static Expression GetMember(this Expression left, MemberInfo member)
		{
			return Expression.MakeMemberAccess(left, member);
		}
		public static Expression GetMember(this Expression left, string member)
		{
			return left.GetMember(
				left.Type.Property(member, BindingFlags.Public | BindingFlags.Instance) ??
				left.Type.Field(member, BindingFlags.Public | BindingFlags.Instance)
				);
		}
		public static Expression SetProperty(this Expression left,PropertyInfo prop,Expression value)
		{
			return Expression.Call(
				left,
				prop.GetSetMethod(),
				value
				);
		}
		public static Expression Get(this MemberInfo member)
		{
			return Expression.MakeMemberAccess(null, member);
		}
		public static Expression LessThan(this Expression left, Expression right)
		{
			return Expression.LessThan(left, right);
		}
		public static Expression LessThanOrEqual(this Expression left, Expression right)
		{
			return Expression.LessThanOrEqual(left, right);
		}
		public static Expression GreaterThan(this Expression left, Expression right)
		{
			return Expression.GreaterThan(left, right);
		}
		public static Expression GreaterThanOrEqual(this Expression left, Expression right)
		{
			return Expression.GreaterThanOrEqual(left, right);
		}
		public static Expression Equal(this Expression left, Expression right)
		{
			return Expression.Equal(left, right);
		}
		public static Expression NotEqual(this Expression left, Expression right)
		{
			return Expression.NotEqual(left, right);
		}
		public static Expression Not(this Expression left)
		{
			return Expression.Not(left);
		}
		public static T Compile<T>(this Expression e, params Expression[] args)
		{
			return Expression.Lambda<T>(e, from a in args select (ParameterExpression)a).Compile();
		}
		public static Delegate Compile(this Expression e, params Expression[] args)
		{
			return Expression.Lambda(e, from a in args select (ParameterExpression)a).Compile();
		}
		public static Expression<T> Lambda<T>(this Expression e, params Expression[] args)
		{
			return Expression.Lambda<T>(e, from a in args select (ParameterExpression)a);
		}
		public static Expression Lambda(this Expression e, params Expression[] args)
		{
			return Expression.Lambda(e, from a in args select (ParameterExpression)a);
		}
		public static IEnumerable<Expression> SelfAndChildren(this Expression e)
		{
			yield return e;
			switch (e.NodeType)
			{
				case ExpressionType.Lambda:
					{
						var mi = (LambdaExpression)e;

						foreach (var c in mi.Parameters)
							yield return c;
						
						foreach (var c in mi.Body.SelfAndChildren())
							yield return c;

						break;
					}
				case ExpressionType.MemberInit:
					{
						var mi = (MemberInitExpression)e;

						foreach (var c in mi.NewExpression.SelfAndChildren())
							yield return c;

						foreach (var b in mi.Bindings)
						{
							switch (b.BindingType)
							{
								case MemberBindingType.Assignment:
									{
										var oe = ((MemberAssignment)b).Expression;
										foreach (var c in oe.SelfAndChildren()) 
											yield return c;
									}
									break;
								case MemberBindingType.ListBinding:
								case MemberBindingType.MemberBinding:
								default:
									throw new NotSupportedException();
							}
						}
						break;
					}
				case ExpressionType.New:
					{
						var ne = (NewExpression)e;
						foreach (var arg in ne.Arguments)
							foreach (var c in arg.SelfAndChildren())
								yield return c;
						break;
					}
				case ExpressionType.NewArrayInit:
					{
						var nae = (NewArrayExpression)e;
						foreach (var arg in nae.Expressions)
							foreach (var c in arg.SelfAndChildren())
								yield return c;
						break;
					}
				case ExpressionType.MemberAccess:
					{
						var ma = (MemberExpression)e;
						foreach (var c in ma.Expression.SelfAndChildren())
							yield return c;
						break;
					}
				case ExpressionType.Call:
					{
						var ce = (MethodCallExpression)e;
						if (ce.Object != null)
							foreach (var c in ce.Object.SelfAndChildren())
								yield return c;
						foreach (var arg in ce.Arguments)
							foreach (var c in arg.SelfAndChildren())
								yield return c;
						break;
					}
				default:
					var be = e as BinaryExpression;
					if (be != null)
					{
						foreach (var c in be.Left.SelfAndChildren())
							yield return c;

						foreach (var c in be.Right.SelfAndChildren())
							yield return c;

					}
					var ue = e as UnaryExpression;
					if (ue != null)
					{
						foreach (var c in ue.Operand.SelfAndChildren())
							yield return c;
					}
					throw new NotSupportedException();
			}
		}

		public static void CollectArguments(this Expression exp, ICollection<ParameterExpression> list)
		{
			Visit(exp, (e, l, i, enter) =>
			{
				if (!enter)
					return true;
				if (e.NodeType != ExpressionType.Parameter)
					return true;
				var pe = (ParameterExpression)e;
				var v = list.FirstOrDefault((ex) => ex.Name == pe.Name);
				if (v == null)
				{
					list.Add(pe);
					return true;
				}
				if (v != pe)
					throw new ArgumentException();
				return true;

			});
		}

		public static bool Visit(this Expression exp, Func<Expression, int, int, bool, bool> visitor)
		{
			return Visit(exp, 0, 0, visitor);
		}

		static bool Visit(Expression exp, int level, int index, Func<Expression, int, int, bool, bool> visitor)
		{
			if (!visitor(exp, level, index, true))
				return false;
			switch (exp.NodeType)
			{
				case ExpressionType.Parameter:
				case ExpressionType.Constant:
					break;
				case ExpressionType.MemberAccess:
					{
						var me = (MemberExpression)exp;
						if (me.Expression != null && !Visit(me.Expression, level + 1, 0, visitor))
							return false;
						break;
					}
				case ExpressionType.MemberInit:
					{
						var mi = (MemberInitExpression)exp;
						Visit(mi.NewExpression, level + 1, 0, visitor);
						var idx = 1;
						foreach (var b in mi.Bindings)
						{
							switch (b.BindingType)
							{
								case MemberBindingType.Assignment:
									Visit(((MemberAssignment)b).Expression, level + 1, idx++, visitor);
									break;
								default:
									throw new NotSupportedException();
							}
						}
						break;
					}
				case ExpressionType.New:
					{
						var ne = (NewExpression)exp;
						var idx = 0;
						var nl = level + 1;
						foreach (var a in ne.Arguments)
							if (!Visit(a, nl, idx++, visitor))
								return false;
						break;
					}
				case ExpressionType.NewArrayInit:
					{
						var ne = (NewArrayExpression)exp;
						var idx = 0;
						var nl = level + 1;
						foreach (var a in ne.Expressions)
							if (!Visit(a, nl, idx++, visitor))
								return false;
						break;
					}
				case ExpressionType.Call:
					{
						var me = (MethodCallExpression)exp;
						var idx = 0;
						var nl = level + 1;
						if (me.Object != null && !Visit(me.Object, nl, idx++, visitor))
							return false;
						foreach (var a in me.Arguments)
							if (!Visit(a, nl, idx++, visitor))
								return false;
						break;
					}
				default:
					var be = exp as BinaryExpression;
					if (be != null)
					{
						if (!Visit(be.Left, level + 1, 0, visitor))
							return false;
						if (!Visit(be.Right, level + 1, 1, visitor))
							return false;
						break;
					}
					var ue = exp as UnaryExpression;
					if (ue != null)
					{
						if (!Visit(ue.Operand, level + 1, 0, visitor))
							return false;
						break;
					}
					throw new NotSupportedException();
			}
			return visitor(exp, level, index, false);
		}
		public static MemberBinding Replace(this MemberBinding b, Func<Expression, Expression> Mapper)
		{
			switch (b.BindingType)
			{
				case MemberBindingType.Assignment:
					{
						var oe = ((MemberAssignment)b).Expression;
						var ne = oe.Replace(Mapper);
						if (oe == ne) return b;
						return Expression.Bind(b.Member, ne);
					}
				case MemberBindingType.ListBinding:
				case MemberBindingType.MemberBinding:
				default:
					throw new NotSupportedException();
			}
		}
		public static Expression Replace(this Expression expr, Func<Expression, Expression> Mapper)
		{
			var e = Mapper(expr);
			if (e == null || e!=expr)
				return e;

			switch (e.NodeType)
			{
				case ExpressionType.Lambda:
					{
						var mi = (LambdaExpression)e;
						var newBody = (NewExpression)Replace(mi.Body, Mapper);
						var newArgs = mi.Parameters.Select(Mapper).Cast<ParameterExpression>().ToArray();
						if (newBody == mi.Body && newArgs.AllEquals(mi.Parameters))
							return e;

						return Expression.Lambda(newBody, newArgs);
					}
				case ExpressionType.MemberInit:
					{
						var mi = (MemberInitExpression)e;
						var newExpr = (NewExpression)Replace(mi.NewExpression, Mapper);
						var binds = mi.Bindings.Select(ie => Replace(ie, Mapper)).ToArray();
						if (newExpr == mi.NewExpression && binds.AllEquals(mi.Bindings))
							return e;
						return Expression.MemberInit(newExpr, binds);
					}
				case ExpressionType.New:
					{
						var ne = (NewExpression)e;
						var args = ne.Arguments.Select(a => a.Replace(Mapper)).ToArray();
						if (args.AllEquals(ne.Arguments))
							return e;
						return Create(ne.Type, args);
					}
				case ExpressionType.NewArrayInit:
					{
						var nae = (NewArrayExpression)e;
						var args = nae.Expressions.Select(a => a.Replace(Mapper)).ToArray();
						if (args.AllEquals(nae.Expressions))
							return e;
						return Expression.NewArrayInit(
							nae.Type.GetElementType(),
							args
							);
					}
				case ExpressionType.MemberAccess:
					{
						var ma = (MemberExpression)e;
						var ne = ma.Expression.Replace(Mapper);
						if (ne == ma.Expression)
							return e;
						return Expression.MakeMemberAccess(ne, ma.Member);
					}
				case ExpressionType.Constant:
					{
						return e;
					}
				case ExpressionType.Call:
					{
						var c = (MethodCallExpression)e;
						var nobj = c.Object == null ? null : c.Object.Replace(Mapper);
						var nargs = c.Arguments.Select(a => a.Replace(Mapper)).ToArray();
						if (nobj == c.Object && nargs.AllEquals(c.Arguments))
							return e;
						return Expression.Call(
							nobj,
							c.Method,
							nargs
							);
					}
				default:

					var be = e as BinaryExpression;
					if (be != null)
					{
						var nl = be.Left.Replace(Mapper);
						var nr = be.Right.Replace(Mapper);
						if (nl == be.Left && nr == be.Right)
							return e;
						return Expression.MakeBinary(
							e.NodeType,
							nl,
							nr,
							be.IsLifted,
							be.Method,
							be.Conversion
							);
					}
					var ue = e as UnaryExpression;
					if (ue != null)
					{
						var nue = ue.Operand.Replace(Mapper);
						if (nue == ue.Operand)
							return e;
						return Expression.MakeUnary(
							e.NodeType,
							nue,
							ue.Type,
							ue.Method
							);
					}
					if (e is ParameterExpression)
						return e;

					throw new NotSupportedException();
			}
		}

		public static Expression Replace(this Expression expr, Dictionary<Expression, Expression> args)
		{
			return expr.Replace(e =>
			{
				if (args.TryGetValue(e, out var ne))
					return ne;
				return e;
			});
		}

		public static MemberBinding Replace(this MemberBinding expr, Dictionary<Expression, Expression> args)
		{
			return expr.Replace(e =>
			{
				if (args.TryGetValue(e, out var ne))
					return ne;
				return e;
			});
		}
		public static Expression ReplaceAndValidateReference(
			this Expression expr, 
			Dictionary<Expression, Expression> args
			)
		{
			return expr.Replace(e =>
			{
				if (args.TryGetValue(e, out var ne))
					return ne;
				var r = e as ConstantExpression;
				if (r != null)
				{
					var type = r.Type;
					if (type.HasElementType)
						type = type.GetElementType();
					if (type.IsGenericType)
					{
						var gt = type.GetGenericTypeDefinition();
						if (gt == typeof(IEnumerable<>) || gt == typeof(Dictionary<,>) || gt == typeof(List<>))
							type = gt.GetGenericArguments().Last();
					}
					if (!type.IsConstType())
						throw new NotSupportedException($"表达式不支持类型为{r.Type}的引用:{r.Value}");
				}
				return e;
			});
		}

		
	}
}
