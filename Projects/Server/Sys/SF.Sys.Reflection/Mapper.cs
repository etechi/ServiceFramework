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
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using SF.Sys.Reflection;

namespace System.Reflection
{
	public delegate Expression ExpressionCopyMapper(Expression Src, Expression Dst);
	public delegate Expression ExpressionCloneMapper(Expression Src);

	public delegate void FuncCopyMapper<S,D>(S Src, D Dst);
	public delegate D FuncCloneMapper<S,D>(S Src);


	public interface IMapperCollection
	{
		void Add(Type SrcType,Type DstType, Func<IMapperProvider,ExpressionCopyMapper> Mapper);
		void Add(Type SrcType, Type DstType, Func<IMapperProvider, ExpressionCloneMapper> Mapper);
		void Add<S,D>(Func<IMapperProvider, FuncCopyMapper<S,D>> Mapper);
		void Add<S,D>(Func<IMapperProvider, FuncCloneMapper<S,D>> Mapper);
	}
	public interface IMapperProvider
	{
		ExpressionCopyMapper FindExpressionCopyMapper(Type SrcType, Type DstType);
		ExpressionCloneMapper FindExpressionCloneMapper(Type SrcType, Type DstType);

		FuncCopyMapper<S, D> FindFuncCopyMapper<S, D>();
		FuncCloneMapper<S, D> FindFuncCloneMapper<S, D>();
	}

	enum MapperEntryType
	{
		ExpressionCopyMapper,
		ExpressionCloneMapper,
		FuncCopyMapper,
		FuncCloneMapper
	}
	class MapperProvider :
		Dictionary<(Type, Type, MapperEntryType), Lazy<object>>,
		IMapperProvider
	{
		public ExpressionCloneMapper FindExpressionCloneMapper(Type SrcType, Type DstType)
		{
			return TryGetValue((SrcType, DstType, MapperEntryType.ExpressionCloneMapper), out var m) ? (ExpressionCloneMapper)m.Value : null;
		}

		public ExpressionCopyMapper FindExpressionCopyMapper(Type SrcType, Type DstType)
		{
			return TryGetValue((SrcType, DstType, MapperEntryType.ExpressionCopyMapper), out var m) ? (ExpressionCopyMapper)m.Value : null;
		}

		public FuncCloneMapper<S, D> FindFuncCloneMapper<S, D>()
		{
			return TryGetValue((typeof(S), typeof(D), MapperEntryType.FuncCloneMapper), out var m) ? (FuncCloneMapper<S, D>)m.Value : null;
		}

		public FuncCopyMapper<S, D> FindFuncCopyMapper<S, D>()
		{
			return TryGetValue((typeof(S), typeof(D), MapperEntryType.ExpressionCloneMapper), out var m) ? (FuncCopyMapper<S, D>)m.Value : null;
		}
	}
	public class MapperCollection :
		IMapperCollection
	{
		internal List<(Type SrcType, Type DstType, MapperEntryType EntryType, Lazy<object> Mapper)> Entries { get; } = new List<(Type SrcType, Type DstType, MapperEntryType EntryType, Lazy<object> Mapper)>();
		MapperProvider Provider { get; } = new MapperProvider();
		public void Add(Type SrcType, Type DstType, Func<IMapperProvider,ExpressionCopyMapper> Mapper)
		{
			Entries.Add((SrcType, DstType, MapperEntryType.ExpressionCopyMapper, new Lazy<object>(()=>Mapper(Provider))));
		}

		public void Add(Type SrcType, Type DstType, Func<IMapperProvider, ExpressionCloneMapper> Mapper)
		{
			Entries.Add((SrcType, DstType, MapperEntryType.ExpressionCloneMapper, new Lazy<object>(() => Mapper(Provider))));
		}

		public void Add<S, D>(Func<IMapperProvider, FuncCopyMapper<S, D>> Mapper)
		{
			Entries.Add((typeof(S), typeof(D), MapperEntryType.FuncCopyMapper, new Lazy<object>(() => Mapper(Provider))));
		}

		public void Add<S, D>(Func<IMapperProvider, FuncCloneMapper<S, D>> Mapper)
		{
			Entries.Add((typeof(S), typeof(D), MapperEntryType.FuncCloneMapper, new Lazy<object>(() => Mapper(Provider))));
		}
	}
	public static class MapperProviderBuilder
	{
	
		public static IMapperProvider Build(IMapperCollection Collection)
		{
			var mp = new MapperProvider();
			foreach (var e in ((MapperCollection)Collection).Entries)
				mp.Add((e.SrcType, e.DstType, e.EntryType), e.Mapper);
			return mp;
		}
	}
	public class ExprParameter
	{
		protected static int Index = 0;
	}
	public class ExprParameter<T> : ExprParameter
	{
		ExprParameter() { }
		public static ParameterExpression Expression { get; } = System.Linq.Expressions.Expression.Parameter(typeof(T), "arg" + (Index++));
	}
	public static class MapperCollectionExtension
	{
		public static void AddConstTypeExpressionCloneMapper(this IMapperCollection mc,Type Type)
			=> mc.Add(Type, Type, ps=>src=>src);


		static Expression CopyProperty(Expression Src, PropertyInfo prop, IMapperProvider Provider)
		{
			var v = Expression.Property(
							 Src,
							 prop
							 );
			var mapper = Provider.FindExpressionCloneMapper(Src.Type, Src.Type);
			if (mapper == null)
				throw new ArgumentException($"未定义{Src.Type}->{Src.Type}的表达式克隆映射器");
			return mapper(v);
		}

		static Expression NewExpression(Type Type,Expression src,IMapperProvider ps) =>
			Expression.MemberInit(
				Expression.New(Type),
				(from prop in Type.AllPublicInstanceProperties()
				 select Expression.Bind(
					 prop,
					 CopyProperty(src, prop, ps)
					 )
				)
			);
		static Expression CopyExpression(Type Type, Expression src, Expression dst, IMapperProvider ps) =>
			Expression.Block(
				Type.AllPublicInstanceProperties().Select(prop =>
					Expression.Call(
						dst,
						prop.SetMethod,
						CopyProperty(src, prop, ps)
					)
				)
			);

		public static void AddPocoClassExpressionCloneMapper(this IMapperCollection mc, Type Type)
			=> mc.Add(
				Type, 
				Type, 
				(ps) =>
					src=>NewExpression(Type,src,ps)
				);

		public static void AddPocoClassExpressionCopyMapper(this IMapperCollection mc, Type Type)
			=> mc.Add(
				Type, 
				Type, 
				ps=>
					(src, dst) => 
						CopyExpression(Type,src,dst,ps)
			);
		public static void AddPocoClassFuncCloneMapper<T>(this IMapperCollection mc)
			=> mc.Add<T, T>(
				ps =>
				Expression.Lambda<FuncCloneMapper<T, T>>(
					NewExpression(
						typeof(Type),
						ExprParameter<T>.Expression,
						ps
						),
					ExprParameter<T>.Expression
					).Compile()
			);
		public static void AddPocoClassFuncCopyMapper<T>(this IMapperCollection mc)
			=> mc.Add<T, T>(
				ps =>
				Expression.Lambda<FuncCopyMapper<T, T>>(
					CopyExpression(
						typeof(Type),
						ExprParameter<T>.Expression,
						ExprParameter<T>.Expression,
						ps
						),
					ExprParameter<T>.Expression
					).Compile()
			);

	}

	//public abstract class MapProviderBase<S, D> : IMapProvider<S, D>
	//{
	//	public static Type SrcType { get; } = typeof(S);
	//	public static Type DstType { get; } = typeof(D);

	//	public Lazy<Expression<Action<S, D>>> LazyCopyExpression { get; }
	//	public Lazy<Expression<Func<S, D>>> LazyCreateExpression { get; }
	//	public Lazy<Action<S, D>> LazyCopyFunc { get; }
	//	public Lazy<Func<S, D>> LazyCreateFunc { get; }

	//	protected abstract Expression<Func<S, D>> OnCreateExpression(ParameterExpression Arg);
	//	protected abstract Expression<Action<S, D>> OnCopyExpression(ParameterExpression ArgSrc, ParameterExpression ArgDst);
	//	public MapProviderBase()
	//	{
	//		LazyCreateExpression = new Lazy<Expression<Func<S, D>>>(() =>
	//			  OnCreateExpression(MapParameter<S>.Expr)
	//		  );
	//		LazyCopyExpression = new Lazy<Expression<Action<S, D>>>(() =>
	//			OnCopyExpression(MapParameter<S>.Expr, MapParameter<D>.Expr)
	//			);
	//		LazyCopyFunc = new Lazy<Action<S, D>>(() => LazyCopyExpression.Value.Compile());
	//		LazyCreateFunc = new Lazy<Func<S, D>>(() => LazyCreateExpression.Value.Compile());
	//	}
	//	public Expression<Action<S,D>> CopyExpression => LazyCopyExpression.Value;

	//	public Expression<Func<S, D>> CreateExpression => LazyCreateExpression.Value;

	//	public Func<S, D> CreateFunc => LazyCreateFunc.Value;

	//	public Action<S, D> CopyFunc => LazyCopyFunc.Value;
	//}
	
	//public class ConstTypeMapProvider : MapProviderBase
	//{
	//	public override bool CanCopy => false;

	//	public override bool CanClone => true;

	//	protected override Expression OnClone(Expression Src, IMapProviderCollection Providers)
	//	{
	//		return Src;
	//	}

	//	protected override Expression OnCopy(Expression Src, Expression Dst, IMapProviderCollection Providers)
	//	{
	//		throw new NotSupportedException();
	//	}
	//}
	//public class ClassMapProvider : MapProviderBase
	//{
	//	public Type Type { get; }
	//	PropertyInfo[] TypeProps { get; } 
	//	public ClassMapProvider(Type Type)
	//	{
	//		this.Type = Type;
	//		TypeProps = Type.AllPublicInstanceProperties();
	//	}

	//	public override bool CanCopy => true;

	//	public override bool CanClone => true;

	//	Expression CopyProperty(Expression Src,PropertyInfo prop, IMapProviderCollection Providers)
	//	{
	//		var v = Expression.Property(
	//						 Src,
	//						 prop
	//						 );
	//		var provider=Providers.FindProvider(Src.Type, Src.Type);
	//		if (provider == null)
	//			throw new ArgumentException($"未定义{Src.Type}->{Src.Type}的映射器");
	//		if(!provider.CanClone)
	//			throw new ArgumentException($"映射器{Src.Type}->{Src.Type}不支持克隆");
	//		return provider.Clone(v,Providers);
	//	}
	//	protected override Expression OnClone(Expression Src, IMapProviderCollection Providers)
	//	{
	//		return Expression.MemberInit(
	//				Expression.New(Type),
	//				(from prop in TypeProps
	//				 select Expression.Bind(
	//					 prop,
	//					 CopyProperty(Src,prop,Providers)
	//					 )
	//				)
	//			);
	//	}

	//	protected override Expression OnCopy(Expression Src, Expression Dst, IMapProviderCollection Providers)
	//	{
	//		return Expression.Block(
	//			TypeProps.Select(prop =>
	//				Expression.Call(
	//					Dst,
	//					prop.SetMethod,
	//					CopyProperty(Src,prop,Providers)
	//				)
	//			)
	//		);
	//	}
	//}
	//public static class Mapper
	//{
	//	class DefaultMapper<T>
	//	{

	//	}
	//	class MapperImpl<S, T>
	//	{
	//		static Type SrcType { get; } = typeof(S);
	//		static Type DstType { get; } = typeof(T);
	//		static PropertyInfo[] dstTypeProps { get; } = DstType.AllPublicInstanceProperties();
	//		static Dictionary<string, PropertyInfo> srcTypeProps { get; } = SrcType.AllPublicInstanceProperties().ToDictionary(p => p.Name);

	//		public static Lazy<Expression<Func<S, T>>> Expr { get; } = new Lazy<Expression<Func<S, T>>>(() =>
	//		{
	//			var arg = Expression.Parameter(typeof(S), "src");
	//			return Expression.Lambda<Func<S, T>>(
	//				Expression.MemberInit(
	//				  Expression.New(DstType),
	//				  (from dp in dstTypeProps
	//				   let sp = srcTypeProps.Get(dp.Name)
	//				   where sp != null
	//				   select Expression.Bind(
	//					   dp,
	//					   Expression.Property(
	//						   arg,
	//						   sp
	//						   )
	//					   )
	//				  )
	//				  ),
	//				arg
	//			  );
	//		});
	//		public static Lazy<Func<S, T>> Func { get; } = new Lazy<Func<S, T>>(() =>
	//			Expr.Value.Compile()
	//		);

	//	}
	//	public static Expression<Func<S, T>> Map<S, T>()
	//	{
	//		return Mapper<S, T>.Expr.Value;
	//	}
	//	public static T Map<S, T>(S src)
	//	{
	//		return Mapper<S, T>.Func.Value(src);
	//	}
	//}
}
