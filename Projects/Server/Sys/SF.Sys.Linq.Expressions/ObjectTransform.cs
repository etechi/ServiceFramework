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
using SF.Sys.Reflection;

namespace SF.Sys.Linq.Expressions
{
	public class ObjectTransform<TContext> 
	{
	
		static MethodInfo StringConcat = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 2 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo StringConcat4 = typeof(String).GetMethods().Single(m => m.Name == "Concat" && m.IsStatic && m.IsPublic && m.GetParameters().Length == 4 && m.GetParameters().All(pt => pt.ParameterType == typeof(string)));
		static MethodInfo Int32ToString = typeof(int).GetMethods().Single(m => m.Name == "ToString" && m.IsPublic && !m.IsStatic && m.GetParameters().Length == 0);

		static Expression StrConcat(Expression e1, Expression e2) =>
			Expression.Call(null, StringConcat, e1, e2);
		public static Expression NullObject { get; } = Expression.Constant((object)null);
		protected virtual bool RequireDynamicPath => false;
		protected virtual Expression CreateEnumerableUserData(
			Expression Source,
			Type ElementType,
			Expression PropPathExpr,
			string PropPath
			)
		{
			return null;
		}
		protected virtual Expression EvalEnumerableItem(
			Expression UserData,
			Expression Index,
			Expression Value,
			Expression PropPathExpr,
			string PropPath
			)
		{
			return EvalExpression(Value,PropPathExpr,PropPath);
		}
		protected virtual Expression EvalEnumerableResult(
			Expression Source,
			Type ElementType,
			Expression PropPathExpr,
			string PropPath,
			Expression UserData,
			bool IsEmptyLoop
			)
		{
			return null;
		}
		protected virtual bool RequireEnumerableIndex => false;
		protected virtual Expression EnumerableEvalExpression(
			Expression Source,
			Type ElementType,
			Expression PropPathExpr,
			string PropPath
			)
		{
			var userDataExpr = CreateEnumerableUserData(Source,ElementType,PropPathExpr,PropPath);
			var idx = RequireEnumerableIndex ? typeof(int).AsVariable() : null;
			var path = RequireDynamicPath ? typeof(string).AsVariable() : null;
			var userData = userDataExpr == null ? null : userDataExpr.Type.AsVariable();

			var args = new List<Expression>();
			var exprs = new List<Expression>();
			if (idx != null) args.Add(idx);
			if (path != null) args.Add(path);
			if (userData != null)
			{
				args.Add(userData);
				exprs.Add(userData.Assign(userDataExpr));
			}
			var isEmptyLoop = false;
			exprs.Add(
				Source.To(typeof(IEnumerable<>).MakeGenericType(ElementType))
					.ForEach(ElementType, item =>
					{
						var es = new List<Expression>();
						if (path!=null)
							es.Add(
								Expression.Assign(
								   path,
								   Expression.Call(
									   null,
									   StringConcat4,
									   PropPathExpr,
									   Expression.Constant("["),
									   idx.CallMethod(Int32ToString),
									   Expression.Constant("]")
									   )
								)
							);
						es.Add(
							EvalEnumerableItem(
							   userData,
							   idx,
							   item, 
							   path??PropPathExpr, 
							   PropPath
							   )
							);
						if(es[es.Count-1]==null)
							isEmptyLoop = true;
						if (idx != null)
							es.Add(Expression.AddAssign(idx, Expression.Constant(1)));
						return es.Count == 1 ? es[0] : Expression.Block(es);
					})
			);

			var re = EvalEnumerableResult(Source, ElementType, PropPathExpr, PropPath, userData, isEmptyLoop);
			if (re != null)
				exprs.Add(re);
			else if (isEmptyLoop)
				return null;

			return args.Count==0 && exprs.Count==1?exprs[0]:
				Expression.Block(
					args.Cast<ParameterExpression>(),
					exprs
				);
		}

		
		protected virtual Expression CreateDictionaryUserData(
			Expression Source, 
			Type KeyType, 
			Type ElementType, 
			Expression PropPathExpr, 
			string PropPath
			)
		{
			return null;
		}
		protected virtual Expression EvalDictionaryItem(
			Expression UserData,
			Expression Key,
			Expression Value,
			Expression PropPathExpr,
			string PropPath
			)
		{
			return EvalExpression(Value, PropPathExpr, PropPath);
		}
		protected virtual Expression EvalDictionaryResult(
			Expression Source,
			Type KeyType,
			Type ElementType,
			Expression PropPathExpr,
			string PropPath,
			Expression UserData,
			bool IsEmptyLoop
			)
		{
			return null;
		}


		protected virtual Expression DictionaryEvalExpression(Expression Source,Type KeyType,Type ElementType,Expression PropPathExpr,string PropPath)
		{
			var userDataExpr = CreateDictionaryUserData(Source, KeyType, ElementType, PropPathExpr, PropPath);

			var path = RequireDynamicPath? typeof(string).AsVariable():null;
			var args = new List<Expression>();
			if (path != null) args.Add(path);

			var exprs = new List<Expression>();
			var userData = userDataExpr == null ? null : userDataExpr.Type.AsVariable();
			if (userData != null)
			{
				args.Add(userData);
				exprs.Add(userData.Assign(userDataExpr));
			}
			var isEmptyLoop = false;
			exprs.Add(
				Source.To(typeof(IEnumerable<>).MakeGenericType(typeof(KeyValuePair<,>).MakeGenericType(KeyType, ElementType)))
				.ForEach(typeof(KeyValuePair<,>).MakeGenericType(KeyType, ElementType), item => {

					var ire = EvalDictionaryItem(
						userData,
						item.GetMember("Key"),
						item.GetMember("Value"),
						PropPathExpr,
						PropPath
						);
					if(ire==null || path==null)
					{
						isEmptyLoop = ire==null;
						return ire ?? NullObject;
					}
					return Expression.Block(
						 Expression.Assign(
							path,
							Expression.Call(
								null,
								StringConcat4,
								PropPathExpr,
								Expression.Constant("["),
								item.GetMember("Key"),
								Expression.Constant("]")
								)
							),
						ire
						);
					}
				)
			);
			var re = EvalDictionaryResult(Source, KeyType, ElementType, PropPathExpr, PropPath, userData,isEmptyLoop);
			if (re != null)
				exprs.Add(re);
			else if (isEmptyLoop)
				return null;

			return args.Count==0 && exprs.Count==1?exprs[0]:
				Expression.Block(
				args.Cast<ParameterExpression>(),
				exprs
			);
		}
		protected virtual Expression EvalPrimitiveExpression(Expression src, Expression PropPathExpr, string PropPath)
		{
			return null;
		}

		protected virtual Expression EvalExpression( Expression src, Expression PropPathExpr, string PropPath)
		{
			var type = src.Type;
			if(type.IsPrimitiveType())
			{
				return EvalPrimitiveExpression(src, PropPathExpr, PropPath);
			}
			else if (type.IsArray)
			{
				return EnumerableEvalExpression(
					src,
					type.GetElementType(),
					PropPathExpr,
					PropPath
				);
			}
			else if (type.IsGeneric())
			{
				var typeArgs = type.GetGenericArguments();
				if (typeArgs.Length == 2 &&
					(typeof(IDictionary<,>).MakeGenericType(typeArgs).IsAssignableFrom(type) ||
					typeof(IReadOnlyDictionary<,>).MakeGenericType(typeArgs).IsAssignableFrom(type)
					)
					)
					return DictionaryEvalExpression(
						src,
						typeArgs[0],
						typeArgs[1],
						PropPathExpr,
						PropPath
					);
				else if (
					typeArgs.Length == 1 &&
					typeof(IEnumerable<>).MakeGenericType(typeArgs).IsAssignableFrom(type)
					)
					return EnumerableEvalExpression(
						src,
						typeArgs[0],
						PropPathExpr,
						PropPath
						);
			}
			return EvalObjectExpression(
				src,
				PropPathExpr,
				PropPath
				);
		}
		protected virtual string GetPropPathName(PropertyInfo Prop) => Prop.Name;
		protected virtual string PathSplitter => ".";
		protected virtual Expression EvalPropExpression(Expression src, Expression PropPathExpr, string PropPath, PropertyInfo Prop)
		{
			return EvalExpression(
				Expression.Property(src, Prop), 
				StrConcat(PropPathExpr, Expression.Constant(PathSplitter + GetPropPathName(Prop))), 
				PropPath + PathSplitter + GetPropPathName(Prop)
				);
		}

		protected virtual Expression[] EvalObjectPropertyExpressions(Expression Src, Expression PropPathExpr, string PropPath)
		{
			var re = Src.Type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
				.Select(pi => EvalPropExpression(Src, PropPathExpr, PropPath, pi)).Where(e => e != null).ToArray();
			return re;
		}

		protected virtual Expression EvalObjectExpression(Expression Src, Expression PropPathExpr, string PropPath)
		{
			var re = EvalObjectPropertyExpressions(Src, PropPathExpr, PropPath);
			if (re.Length == 0)
				return null;
			return re.Length == 1 ? re[0] : Expression.Block(re);
		}
		protected ParameterExpression ArgContext { get; } = Expression.Parameter(typeof(TContext), "Context");
		protected ParameterExpression ArgInstance { get; } = Expression.Parameter(typeof(object), "Instance");
		public Func<object,TContext,object> Build(Type ObjectType)
		{
			return Expression.Lambda<Func<object, TContext, object>>(
				EvalObjectExpression(
					Expression.Convert(ArgInstance, ObjectType),
					Expression.Constant(""),
					""
				) ?? NullObject,
				ArgInstance,
				ArgContext
				).Compile();
		}
	}
}
