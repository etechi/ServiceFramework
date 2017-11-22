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
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading.Tasks;
using SF.Sys.Linq;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;

namespace SF.Sys.Entities.AutoEntityProvider.Internals
{

	public abstract class QueryResultBuildHelper
	{
		public abstract Type TempType { get; }
		public abstract Type DateModelType { get; }
	}

	public class QueryResultBuildHelper<TDataModel,TTemp,TResult>:
		QueryResultBuildHelper,
		IQueryResultBuildHelper<TDataModel,TTemp,TResult>
		where TDataModel:class
		where TResult:class
		where TTemp:class
	{
		Func<Expression, int,Expression> FuncBuildEntityMapper { get; }
		public QueryResultBuildHelper(
			Func<Expression,int,Expression> FuncBuildEntityMapper,
			Lazy<Func<TTemp[], Task<TResult[]>>> ResultMapper
			)
		{
			this.FuncBuildEntityMapper = FuncBuildEntityMapper;
			this.EntityMapper = new Lazy<Expression<Func<TDataModel, TTemp>>>(()=>
			{
				var p = Expression.Parameter(typeof(TDataModel));
				return Expression.Lambda< Func < TDataModel, TTemp >>(FuncBuildEntityMapper(p,0),p);
			});
			this.ResultMapper = ResultMapper;
		}

		public override Type TempType => typeof(TTemp);

		public override Type DateModelType => typeof(TDataModel);

		public Lazy<Expression<Func<TDataModel, TTemp>>> EntityMapper { get; }
		public Lazy<Func<TTemp[], Task<TResult[]>>> ResultMapper { get; }
		public Expression BuildEntityMapper(Expression src,int Level)
		{
			return FuncBuildEntityMapper(src, Level);
		}

		Expression<Func<TDataModel, TTemp>> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.EntityMapper => EntityMapper.Value;

		Func<TTemp[], Task<TResult[]>> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.ResultMapper => ResultMapper.Value;


		public Task<QueryResult<TResult>> Query(IContextQueryable<TDataModel> queryable,IPagingQueryBuilder<TDataModel> PagingQueryBuilder, Paging paging)
		{
			return queryable.ToQueryResultAsync(
				q=>q.Select(EntityMapper.Value),
				ResultMapper.Value,
				PagingQueryBuilder,
				paging,
				null
				);
		}

		public async Task<TResult> QuerySingleOrDefault(IContextQueryable<TDataModel> queryable)
		{
			var re = await queryable.Select(EntityMapper.Value).SingleOrDefaultAsync();
			if (re == null)
				return default(TResult);
			var rre = await ResultMapper.Value(new[] { re });
			return rre[0];
		}
	}
	//public class QueryHelper<TDataModel, TResult> :
	//	QueryResultMapper<TDataModel, TResult, TResult>
	//{
	//	public QueryHelper(
	//		Expression<Func<TDataModel, TResult>> FuncMapModelToTemp, 
	//		Func<TResult[], Task<TResult[]>> FuncMapTempToDetail) : 
	//		base(FuncMapModelToTemp, FuncMapTempToDetail)
	//	{
	//	}
	//}
	public interface ITempModel<TResult>
	{
		TResult __Result { get; set; }
	}
	public class QueryResultBuildHelperCreator
	{
		static MethodInfo MethodOrderByDescending { get; }
		static MethodInfo MethodOrderBy { get; }
		static MethodInfo MethodCreateQueryResultBuildHelper3 { get; }
		static MethodInfo MethodCreateQueryResultBuildHelper2 { get; }

		static QueryResultBuildHelperCreator()
		{
			//var methods = typeof(QueryResultBuildHelperCreator).GetMethods(BindingFlags.Static | BindingFlags.NonPublic);

			MethodOrderByDescending = typeof(ContextQueryable).GetMethodExt(
				  "OrderByDescending",
				  typeof(IContextQueryable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
				  typeof(Expression<>).MakeGenericType<Func<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>>()
				  ).IsNotNull();
			MethodOrderBy = typeof(ContextQueryable).GetMethodExt(
				"OrderBy",
				typeof(IContextQueryable<>).MakeGenericType<TypeExtension.GenericTypeArgument>(),
				typeof(Expression<>).MakeGenericType<Func<TypeExtension.GenericTypeArgument, TypeExtension.GenericTypeArgument>>()
				).IsNotNull();
			MethodCreateQueryResultBuildHelper3 = typeof(QueryResultBuildHelperCreator).GetMethodExt(
				 "CreateQueryResultBuildHelper3",
				 BindingFlags.Static | BindingFlags.NonPublic,
				 typeof(Func<Expression,int,Expression>),
				 typeof(Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IEntityPropertyQueryConverter conv)>>)
				 ).IsNotNull();
			MethodCreateQueryResultBuildHelper2= typeof(QueryResultBuildHelperCreator).GetMethodExt(
				"CreateQueryResultBuildHelper2",
				BindingFlags.Static | BindingFlags.NonPublic,
				typeof(Func<Expression, int, Expression>),
				typeof(Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IEntityPropertyQueryConverter conv)>>)
				).IsNotNull();

		}
		Type SrcType { get; }
		Type DstType { get; }
		List<(PropertyInfo dstProp, PropertyInfo srcProp, IEntityPropertyQueryConverter Converter)> DstBindings { get; } = new List<(PropertyInfo dstProp, PropertyInfo srcProp, IEntityPropertyQueryConverter Converter)>();
		List<(PropertyInfo dstProp, PropertyInfo srcProp, IEntityPropertyQueryConverter Converter)> TempBindings { get; } = new List<(PropertyInfo dstProp, PropertyInfo srcProp, IEntityPropertyQueryConverter Converter)>();
		TypeBuilder TempTypeBuilder;
		Type TempType;
		IEntityPropertyQueryConverterProvider[] PropertyQueryConverterProviders { get; }
		QueryMode QueryMode { get; }
		int Level { get; }
		static AssemblyBuilder AssemblyBuilder { get; } =
			AssemblyBuilder.DefineDynamicAssembly(
			new AssemblyName("SFAutoEntityProviderQueryTempClasses"),
			AssemblyBuilderAccess.Run
		);
		static ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(new Guid().ToString("N"));


		PropertyBuilder BuildProperty(TypeBuilder typeBuilder, string name, Type type)
		{
			var field = typeBuilder.DefineField("_" + name, type, FieldAttributes.Private);
			var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, null);

			var getSetAttr = MethodAttributes.Public |
							MethodAttributes.HideBySig |
							MethodAttributes.SpecialName |
							MethodAttributes.Virtual;

			var getter = typeBuilder.DefineMethod("get_" + name, getSetAttr, type, Type.EmptyTypes);
			var getIL = getter.GetILGenerator();
			getIL.Emit(OpCodes.Ldarg_0);
			getIL.Emit(OpCodes.Ldfld, field);
			getIL.Emit(OpCodes.Ret);

			var setter = typeBuilder.DefineMethod("set_" + name, getSetAttr, null, new Type[] { type });

			var setIL = setter.GetILGenerator();
			setIL.Emit(OpCodes.Ldarg_0);
			setIL.Emit(OpCodes.Ldarg_1);
			setIL.Emit(OpCodes.Stfld, field);
			setIL.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(getter);
			propertyBuilder.SetSetMethod(setter);
			return propertyBuilder;
		}
		static int TempClassIdSeed = 1;
		static int GetTempClassIdSeed()
		{
			return System.Threading.Interlocked.Increment(ref TempClassIdSeed);
		}
		void AddTempProperty(string Name,Type Type)
		{
			if (TempTypeBuilder == null)
			{
				TempTypeBuilder = ModuleBuilder.DefineType(
					DstType.Name + "_Temp"+ GetTempClassIdSeed(),
					TypeAttributes.Public | TypeAttributes.Class
					);
				TempTypeBuilder.AddInterfaceImplementation(typeof(ITempModel<>).MakeGenericType(DstType));
				BuildProperty(TempTypeBuilder, "__Result", DstType);
			}
			BuildProperty(TempTypeBuilder, Name, Type);
		}
		public QueryResultBuildHelperCreator(
			Type SrcType,
			Type DstType, 
			QueryMode Mode,
			IEntityPropertyQueryConverterProvider[] PropertyQueryConverterProviders
			)
		{
			this.QueryMode = Mode;
			this.SrcType = SrcType;
			this.DstType = DstType;
			this.PropertyQueryConverterProviders = PropertyQueryConverterProviders;
		}
		
		
		IEntityPropertyQueryConverter FindValueConverter(PropertyInfo srcProp, PropertyInfo dstProp)
		{
			return PropertyQueryConverterProviders
				.Select(p => p.GetPropertyConverter(SrcType, srcProp,DstType, dstProp, QueryMode))
				.FirstOrDefault(c => c != null);
		}

		void BuildBindings(PropertyInfo dstProp)
		{
			var srcProp=SrcType.GetProperty(dstProp.Name, BindingFlags.Instance | BindingFlags.Public);
			var vc = FindValueConverter(srcProp, dstProp);
			if (vc == null)
				return;
			
			if(vc.TempFieldType==null)
			{
				DstBindings.Add((dstProp, srcProp, vc));
			}
			else
			{
				AddTempProperty(dstProp.Name, vc.TempFieldType);
				TempBindings.Add((dstProp,srcProp,vc));
			}
			
	
		}

		class Preparer<TTemp, TResult>
		{
			public static async Task RunTasks(TTemp temp, TResult result, Func<TTemp, TResult, Task, Task>[] funcs)
			{
				Task lastTask = null;
				foreach (var f in funcs)
				{
					var t = f(temp, result, lastTask);
					if (t == null)
						break;
					await t;
					lastTask = t;
				}
			}
			static MethodInfo MethodRunTasks { get; } = typeof(Preparer<TTemp, TResult>).GetMethod("RunTasks", BindingFlags.Static | BindingFlags.Public);

			public static Func<TTemp, TResult, Task, Task>[] BuildPrepare(
				IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IEntityPropertyQueryConverter conv)> prepares
				)
			{
				var argTemp = Expression.Parameter(typeof(TTemp));
				var argResult = Expression.Parameter(typeof(TResult));
				var argTask = Expression.Parameter(typeof(Task));
				var funcs = new List<Func<TTemp, TResult, Task, Task>>();
				var exprs = new List<Expression>();
				foreach (var p in prepares)
				{
					var convType = typeof(IEntityPropertyQueryConverter<,>).MakeGenericType(p.propTemp.PropertyType, p.prop.PropertyType);
					if (!convType.IsAssignableFrom(p.conv.GetType()))
					{
						convType = typeof(IEntityPropertyQueryConverterAsync<,>).MakeGenericType(p.propTemp.PropertyType, p.prop.PropertyType);
						if (!convType.IsAssignableFrom(p.conv.GetType()))
							throw new NotSupportedException();
					}
					var expr = Expression.Call(
					   Expression.Convert(
						   Expression.Constant(p.conv),
						   convType
						   ),
					   convType.GetMethod("TempToDest", BindingFlags.Public | BindingFlags.Instance),
					   argTemp,
					   p.propTemp == null ?
					   (Expression)Expression.Constant(
						   p.propTemp.PropertyType.GetDefaultValue(),
						   p.propTemp.PropertyType
						   ) :
					   (Expression)Expression.Property(argTemp, p.propTemp)
					   );
					if (!expr.Type.IsGenericTypeOf(typeof(Task<>)))
					{
						exprs.Add(argResult.SetProperty(p.prop, expr));
						continue;
					}
					else
						exprs.Add(expr);
					funcs.Add(
						Expression.Block(exprs).Compile<Func<TTemp, TResult, Task, Task>>(
							argTemp,
							argResult,
							argTask
							)
						);
					exprs.Clear();
					exprs.Add(argResult.SetProperty(
						p.prop, 
						argTask.To(typeof(Task<>).MakeGenericType(p.prop.PropertyType)).GetMember(nameof(Task<int>.Result))
						)
					);
				}
				exprs.Add(Expression.Constant(null, typeof(Task)));
				funcs.Add(
					Expression.Block(exprs).Compile<Func<TTemp, TResult, Task, Task>>(
						argTemp,
						argResult,
						argTask
						)
					);
				return funcs.ToArray();
			}
		}

		//	return Expression.Lambda<Func<TTemp, TResult, Task[]>>(
		//		Expression.NewArrayInit(
		//			typeof(Task),
		//			prepares.Select(p =>
		//			{
		//				var convType = typeof(IEntityPropertyQueryConverter<,>).MakeGenericType(p.propTemp.PropertyType, p.prop.PropertyType);

		//				var converterCall = Expression.Call(
		//					Expression.Convert(
		//						Expression.Constant(p.conv),
		//						convType
		//						),
		//					convType.GetMethod("TempToDest", BindingFlags.Public | BindingFlags.Instance),
		//					argTemp,
		//					p.propTemp == null ?
		//					(Expression)Expression.Constant(
		//						p.propTemp.PropertyType.GetDefaultValue(),
		//						p.propTemp.PropertyType
		//						) :
		//					(Expression)Expression.Property(argTemp, p.propTemp)
		//				);
		//				var taskType = typeof(Task<>).MakeGenericType(p.prop.PropertyType);
		//				var argTask = Expression.Parameter(typeof(Task<>).MakeGenericType(p.prop.PropertyType));
		//				var argContext = Expression.Parameter(typeof(object));
		//				var bind = Expression.Lambda(
		//					Expression.Call(
		//						Expression.Convert(argContext, typeof(TResult)),
		//						p.prop.GetSetMethod(),
		//						Expression.Property(
		//							argTask,
		//							argTask.Type.GetProperty("Result")
		//							)
		//						),
		//					argTask,
		//					argContext
		//					).Compile();
		//				return Expression.Convert(
		//					Expression.Call(
		//						converterCall,
		//						taskType.GetMethod(
		//							"ContinueWith",
		//							BindingFlags.Public | BindingFlags.Instance,
		//							null,
		//							new[] {
		//									typeof(Action<,>)
		//									.MakeGenericType(
		//										typeof(Task<>).MakeGenericType(p.prop.PropertyType),
		//										typeof(object)
		//										),
		//									typeof(object)
		//							},
		//							null
		//						),
		//						Expression.Constant(bind),
		//						argResult
		//						),
		//					typeof(Task)
		//					);
		//			}
		//		)), argTemp, argResult).Compile();
		//}
		
	

		static QueryResultBuildHelper CreateQueryResultBuildHelper3<TDataModel, TTemp, TResult>(
			Func<Expression,int,Expression> EntityMapper,
			Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IEntityPropertyQueryConverter conv)>> prepares
			) where TTemp : class,ITempModel<TResult>
			where TDataModel:class
			where TResult:class
		{
			return new QueryResultBuildHelper<TDataModel, TTemp, TResult>(
				EntityMapper,
				new Lazy<Func<TTemp[], Task<TResult[]>>>(() => {
					var fs =  Preparer<TTemp, TResult>.BuildPrepare(prepares());
					return async (tmps) =>
					{
						foreach (var t in tmps)
							await Preparer<TTemp, TResult>.RunTasks(t, t.__Result, fs);
						return tmps.Select(t => t.__Result).ToArray();
					};
				}
				)
				);
		}

		static QueryResultBuildHelper CreateQueryResultBuildHelper2<TDataModel, TResult>(
			Func<Expression,int,Expression> EntityMapper,
			Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IEntityPropertyQueryConverter conv)>> prepares
			)
			where TDataModel:class
			where TResult:class
		{
			return new QueryResultBuildHelper<TDataModel, TResult, TResult>(
					EntityMapper,
					new Lazy<Func<TResult[], Task<TResult[]>>>(() => {
						//var fs = Preparer<TResult, TResult>.BuildPrepare(prepares());
						return (tmps) =>
						{
							//foreach (var t in tmps)
								//await Preparer<TResult, TResult>.RunTasks(t, t, fs);
							return Task.FromResult(tmps);
						};
					})
				);
		}
		
		public QueryResultBuildHelper Build()
		{
			foreach(var prop in DstType.AllPublicInstanceProperties())
				BuildBindings(prop);

			if (TempTypeBuilder != null)
			{
				TempType = TempTypeBuilder.CreateTypeInfo().AsType();

				return (QueryResultBuildHelper)MethodCreateQueryResultBuildHelper3
					.MakeGenericMethod(SrcType, TempType, DstType).Invoke(
					null,
					new object[] {
						new Func<Expression,int,Expression>((arg,level)=>
							Expression.MemberInit(
								Expression.New(TempType),
								(from b in TempBindings
								let e=b.Converter.SourceToDestOrTemp(arg,level,b.srcProp,b.dstProp)
								where e!=null
								select Expression.Bind(TempType.GetProperty(b.dstProp.Name), e)
								)
								.WithFirst(
									Expression.Bind(
										TempType.GetProperty("__Result"),
										Expression.MemberInit(
											Expression.New(DstType),
											from b in DstBindings
											let e=b.Converter.SourceToDestOrTemp(arg,level,b.srcProp,b.dstProp)
											where e!=null
											select Expression.Bind(b.dstProp,e)
										)
									)
								)
						)),
						new Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IEntityPropertyQueryConverter conv)>>(
							()=>TempBindings.Select(p => (prop:p.dstProp,propTemp: TempType.GetProperty(p.dstProp.Name), conv:p.Converter))
						)
					}
					);
			}
			else
			{
				return (QueryResultBuildHelper)MethodCreateQueryResultBuildHelper2
					.MakeGenericMethod(SrcType,  DstType).Invoke(
					null,
					new object[] {
						new Func<Expression,int,Expression>((arg,level)=>
							Expression.MemberInit(
								Expression.New(DstType),
								from b in DstBindings
									let e=b.Converter.SourceToDestOrTemp(arg,level,b.srcProp,b.dstProp)
									where e!=null
									select Expression.Bind(b.dstProp,e)
							)
						),
						null
						//new Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IEntityPropertyQueryConverter conv)>>(
						//	()=>Converters.Select(p => (prop:p.prop,propTemp: p.prop, conv:p.conv))
						//)
					}
					);
			}
		}
	}
}
