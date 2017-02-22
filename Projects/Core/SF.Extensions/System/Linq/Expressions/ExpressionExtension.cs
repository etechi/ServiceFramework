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
			return Create(type,(IEnumerable<Expression>) args);
		}
		public static Expression Create(this Type type,IEnumerable<Expression> args)
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
		public static Expression Assign(this Expression left,Expression right)
		{
			return Expression.Assign(left, right);
		}
		public static Expression CallMethod(this Expression obj,MethodInfo method,params Expression[] args)
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
			return CallMethod(obj, obj.Type.FindMethod(name,BindingFlags.Public | BindingFlags.Instance), args.ToArray());
		}
		public static MethodInfo FindMethod(this Type type, string name, BindingFlags flags)
		{
			var m = type.GetMethod(name, flags);
			if (m != null)
				return m;
			foreach (var t in type.GetInterfaces())
			{
				m = t.FindMethod(name,flags);
				if (m != null)
					return m;
			}
			return null;

				//type.method(name, BindingFlags.NonPublic | BindingFlags.Instance) ??
				//type.method(name, BindingFlags.NonPublic | BindingFlags.Static);
		}
		public static MethodInfo Method(this Type type,string name,System.Reflection.BindingFlags flags)
		{
			return type.GetMethod(name, flags);
		}
		public static MethodInfo Method(this Type type, string name,params Type[] types)
		{
			name += "<" + new string(',', types.Length - 1) + ">";
			var m=
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
				type.Property(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
				type.Property(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
				type.Field(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField) ??
				type.Field(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField);
				/*type.prop(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
				type.prop(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetProperty | BindingFlags.SetProperty) ??
				type.field(name, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField) ??
				type.field(name, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.GetField | BindingFlags.SetField);
				 */
		}
		public static MemberInfo Property(this Type type,string name,BindingFlags flags)
		{
			return type.GetProperties(flags).FirstOrDefault(p=>p.Name==name);
		}
		public static MemberInfo Field(this Type type, string name, BindingFlags flags)
		{
			return type.GetField(name, flags);
		}
		
		public static Expression Loop(Expression body,LabelTarget break_target=null,LabelTarget continue_target=null)
		{
			return Expression.Loop(body, break_target, continue_target);
		}
		public static Expression Then(this Expression expr,params Expression[] true_expr)
		{
			return true_expr.Length == 1 ?
				Expression.IfThen(expr, true_expr[0]) :
				Expression.IfThen(expr, Block(true_expr));
		}
		public static Expression Then(this Expression expr,IEnumerable<Expression> true_expr)
		{
			return Then(expr, true_expr.ToArray());
		}
		public static Expression Then(this Expression expr, Expression true_expr,Expression false_expr)
		{
			return Expression.IfThenElse(expr, true_expr, false_expr);
		}
		public static Expression ThenLoop(this Expression cond,params Expression[] body)
		{
			var bl = Expression.Label();
			var b=body.Length==1?body[0]:Block(body);
			return Loop(cond.Then(b, Expression.Break(bl)), bl);
		}

		public static Expression To(this Expression v,Type type)
		{
			return Expression.Convert(v, type);
		}
		public static Expression To<T>(this Expression v)
		{
			return v.To(typeof(T));
		}
		public static Expression UseEnumerator<T>(this Expression e,Func<Expression,Expression> body)
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
		public static Type MakeGenericType<T1,T2>(this Type type)
		{
			return type.MakeGenericType(typeof(T1),typeof(T2));
		}
		public static Type MakeGenericType<T1, T2, T3>(this Type type)
		{
			return type.MakeGenericType(typeof(T1), typeof(T2),typeof(T3));
		}
		public static Type MakeGenericType<T1, T2, T3,T4>(this Type type)
		{
			return type.MakeGenericType(typeof(T1), typeof(T2), typeof(T3),typeof(T4));
		}
		public static Expression UseEnumerator(this Expression e, Type type,Func<Expression, Expression> body)
		{
			return e.To(typeof(IEnumerable<>).MakeGenericType(type)).CallMethod("GetEnumerator").Use(body);
		}
		public static Expression Iterator(this Expression e, Type type,Func<Expression, Expression> body)
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
		public static Expression Iterator(this Expression e, ParameterExpression arg,Func<Expression, Expression> body)
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
		public static Expression ForEach(this Expression e,Type type,Func<Expression, Expression> body)
		{
			return e.UseEnumerator(type, ie => ie.Iterator(type, body));
		}
		public static Expression ForEachEach(this Expression e,ParameterExpression arg,Func<Expression, Expression> body)
		{
			return e.UseEnumerator(arg.Type, ie => ie.Iterator(arg, body));
		}
		public static Expression Use(this Expression e,Func<Expression,Expression> body)
		{
			var v = e.Type.AsVariable();
			return Expression.Block(
				new ParameterExpression[]{(ParameterExpression)v},
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
				method,
				args
				);
		}
		public static Expression AsParameter(this Type type,string name)
		{
			return Expression.Parameter(type,name);
		}

		public static Expression AsVariable(this Type type, string name=null)
		{
			return Expression.Variable(type, name);
		}
		public static Expression AsVariable<T>(string name=null)
		{
			return AsVariable(typeof(T), name);
		}
		
		public static Expression Add(this Expression left,Expression right)
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
				left.Type.Property(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty) ??
				left.Type.Field(member, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetField)
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
		public static T Compile<T>(this Expression e,params Expression[] args)
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
						foreach(var b in mi.Bindings)
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
	
	}
}
