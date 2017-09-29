using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using SF.Metadata;
using System.Reflection.Emit;
using SF.Core.ServiceManagement;
using System.ComponentModel.DataAnnotations.Schema;
using SF.Data;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SF.Entities.AutoEntityProvider.Internals
{

	public abstract class QueryResultBuildHelper
	{
		public abstract Type TempType { get; }
		public abstract Type DateModelType { get; }
	}

	public class QueryResultBuildHelper<TDataModel,TTemp,TResult>:
		QueryResultBuildHelper,
		IQueryResultBuildHelper<TDataModel,TTemp,TResult>
	{
		public QueryResultBuildHelper(
			Lazy<Expression<Func<TDataModel, TTemp>>> EntityMapper,
			Lazy<Func<TTemp[], Task<TResult[]>>> ResultMapper,
			Lazy<IPagingQueryBuilder<TDataModel>> PagingBuilder,
			Lazy<Expression<Func<IGrouping<int, TDataModel>, ISummaryWithCount>>> Summary
			)
		{
			this.EntityMapper = EntityMapper;
			this.ResultMapper = ResultMapper;
			this.PagingBuilder = PagingBuilder;
			this.Summary = Summary;
		}

		public override Type TempType => typeof(TTemp);

		public override Type DateModelType => typeof(TDataModel);

		public Lazy<Expression<Func<TDataModel, TTemp>>> EntityMapper { get; }
		public Lazy<Func<TTemp[], Task<TResult[]>>> ResultMapper { get; }
		public Lazy<IPagingQueryBuilder<TDataModel>> PagingBuilder { get; }
		public Lazy<Expression<Func<IGrouping<int, TDataModel>, ISummaryWithCount>>> Summary { get; }

		Expression<Func<TDataModel, TTemp>> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.EntityMapper => EntityMapper.Value;

		Func<TTemp[], Task<TResult[]>> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.ResultMapper => ResultMapper.Value;

		IPagingQueryBuilder<TDataModel> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.PagingBuilder => PagingBuilder.Value;

		Expression<Func<IGrouping<int, TDataModel>, ISummaryWithCount>> IQueryResultBuildHelper<TDataModel, TTemp, TResult>.Summary => Summary.Value;
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
	class BaseTempModel<TResult> 
	{
		public TResult __Result { get; set; }
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
				  typeof(IContextQueryable<>).MakeGenericType<TypeExtension.T>(),
				  typeof(Expression<>).MakeGenericType<Func<TypeExtension.T, TypeExtension.T>>()
				  ).IsNotNull();
			MethodOrderBy = typeof(ContextQueryable).GetMethodExt(
				"OrderBy",
				typeof(IContextQueryable<>).MakeGenericType<TypeExtension.T>(),
				typeof(Expression<>).MakeGenericType<Func<TypeExtension.T, TypeExtension.T>>()
				).IsNotNull();
			MethodCreateQueryResultBuildHelper3 = typeof(QueryResultBuildHelperCreator).GetMethodExt(
				 "CreateQueryResultBuildHelper3",
				 BindingFlags.Static | BindingFlags.NonPublic,
				 typeof(Func<Expression>),
				 typeof(Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)>>)
				 ).IsNotNull();
			MethodCreateQueryResultBuildHelper2= typeof(QueryResultBuildHelperCreator).GetMethodExt(
				"CreateQueryResultBuildHelper2",
				BindingFlags.Static | BindingFlags.NonPublic,
				typeof(Func<Expression>),
				typeof(Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)>>)
				).IsNotNull();

		}
		Type SrcType { get; }
		Type DstType { get; }
		ParameterExpression ArgSource { get; }
		List<MemberBinding> DstBindings { get; } = new List<MemberBinding>();
		List<(string, Expression)> TempBindings { get; } = new List<(string, Expression)>();
		List<(PropertyInfo prop, IValueConverter conv)> Converters { get; } = new List<(PropertyInfo prop, IValueConverter conv)>();
		TypeBuilder TempTypeBuilder;
		Type TempType;
		NamedServiceResolver<IValueConverter> ValueConverterResolver { get; }

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
		void AddTempProperty(string Name,Type Type)
		{
			if (TempTypeBuilder == null)
				TempTypeBuilder = ModuleBuilder.DefineType(
					DstType.Name + "_Temp", 
					TypeAttributes.Public | TypeAttributes.Class, 
					typeof(BaseTempModel<>).MakeGenericType(DstType)
					);
			BuildProperty(TempTypeBuilder, Name, Type);
		}
		public QueryResultBuildHelperCreator(Type SrcType,Type DstType, NamedServiceResolver<IValueConverter> ValueConverterResolver)
		{
			this.SrcType = SrcType;
			this.DstType = DstType;
			this.ValueConverterResolver = ValueConverterResolver;
			this.ArgSource = Expression.Parameter(SrcType);
		}
		
		
		IValueConverter FindValueConverter(PropertyInfo srcProp,PropertyInfo dstProp)
		{
			if (ValueConverterResolver == null)
				return null;
			IValueConverter re = null;

			if (srcProp != null && null != (re = ValueConverterResolver(
				$"{srcProp.DeclaringType.FullName}:{srcProp.Name}->{dstProp.DeclaringType.FullName}:{dstProp.Name}"
				)))
				return re;

			if (srcProp != null && null != (re = ValueConverterResolver(
				$"{srcProp.PropertyType.FullName}->{dstProp.PropertyType.FullName}"
				)))
				return re;
			if (null != (re = ValueConverterResolver($"{dstProp.PropertyType.FullName}")))
				return re;
			return null;
		}

		void BuildBindings(PropertyInfo dstProp)
		{
			var srcProp=SrcType.GetProperty(dstProp.Name, BindingFlags.Instance | BindingFlags.Public);
			var vc = FindValueConverter(srcProp, dstProp);
			if (vc != null)
			{
				if(vc.TempFieldType!=null)
				{
					AddTempProperty(dstProp.Name, vc.TempFieldType);
					TempBindings.Add((dstProp.Name, vc.SourceToTemp(ArgSource, srcProp)));
				}
				Converters.Add((dstProp, vc));
			}
			else if (srcProp == null)
				return;
			else
			{
				var src = (Expression)Expression.Property(ArgSource, srcProp);
 				if (dstProp.PropertyType != srcProp.PropertyType)
				{
					if (srcProp.PropertyType.CanSimpleConvertTo(dstProp.PropertyType))
						src = Expression.Convert(src, dstProp.PropertyType);
					else
						throw new NotSupportedException($"来源字段{SrcType.FullName}.{srcProp.Name} 的类型{srcProp.PropertyType} 和目标字段 {DstType.FullName}.{dstProp.Name} 的类型不兼容");
				}
				DstBindings.Add(Expression.Bind(dstProp, src));
			}
		}

		static Func<TTemp, TResult, Task[]> BuildPrepare<TTemp, TResult>(
			IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)> prepares
			)
		{
			var argTemp = Expression.Parameter(typeof(TTemp));
			var argResult = Expression.Parameter(typeof(TResult));

			return Expression.Lambda<Func<TTemp, TResult, Task[]>>(
				Expression.NewArrayInit(
					typeof(Task),
					prepares.Select(p =>
					{
						var convType = typeof(IValueConverter<,>).MakeGenericType(p.propTemp.PropertyType, p.prop.PropertyType);

						var converterCall = Expression.Call(
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
						var taskType = typeof(Task<>).MakeGenericType(p.prop.PropertyType);
						var argTask = Expression.Parameter(typeof(Task<>).MakeGenericType(p.prop.PropertyType));
						var argContext = Expression.Parameter(typeof(object));
						var bind = Expression.Lambda(
							Expression.Call(
								Expression.Convert(argContext, typeof(TResult)),
								p.prop.GetSetMethod(),
								Expression.Property(
									argTask,
									argTask.Type.GetProperty("Value")
									)
								),
							argTask,
							argContext
							).Compile();
						return Expression.Convert(
							Expression.Call(
								converterCall,
								taskType.GetMethod(
									"ContinueWith",
									BindingFlags.Public | BindingFlags.Instance,
									null,
									new[] {
											typeof(Action<,>)
											.MakeGenericType(
												typeof(Task<>).MakeGenericType(p.prop.PropertyType),
												typeof(object)
												),
											typeof(object)
									},
									null
								),
								Expression.Constant(bind),
								argResult
								),
							typeof(Task)
							);
					}
				)), argTemp, argResult).Compile();
		}
		
		static IPagingQueryBuilder<T> BuildPagingQueryBuilder<T>()
		{
			var id = typeof(T).AllPublicInstanceProperties().Where(p => p.IsDefined(typeof(KeyAttribute))).FirstOrDefault();
			var argQueryable = Expression.Parameter(typeof(IContextQueryable<T>));
			var argDesc = Expression.Parameter(typeof(bool));
			var argEntity = Expression.Parameter(typeof(T));
			var IdMapper = Expression.Lambda(
				Expression.Property(argEntity,id),
				argEntity
				);
			var KeySelectorType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(T), id.PropertyType));

			var expr = Expression.Lambda<Func<IContextQueryable<T>,bool, IContextQueryable<T>>>(
				Expression.Condition(
					argDesc,
					Expression.Call(
						null,
						MethodOrderByDescending.MakeGenericMethod(
							typeof(IContextQueryable<>).MakeGenericType<T>(),
							KeySelectorType
							),
						argQueryable,
						Expression.Constant(
							IdMapper,
							KeySelectorType
							)
						),
					Expression.Call(
						null,
						MethodOrderBy.MakeGenericMethod(
							typeof(IContextQueryable<>).MakeGenericType<T>(),
							KeySelectorType
							),
						argQueryable,
						Expression.Constant(
							IdMapper,
							KeySelectorType
							)
						)
				),
				argQueryable,
				argDesc
				).Compile();

			return new PagingQueryBuilder<T>(
				id.Name, 
				b => b.Add(
					id.Name,
					expr, 
					true
					));
		}

		static QueryResultBuildHelper CreateQueryResultBuildHelper3<TDataModel, TTemp, TResult>(
			Func<Expression> Expr,
			Func<IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)>> prepares
			) where TTemp : BaseTempModel<TResult>
		{
			return new QueryResultBuildHelper<TDataModel, TTemp, TResult>(
				new Lazy<Expression<Func<TDataModel, TTemp>>>(() => (Expression<Func<TDataModel, TTemp>>)Expr()),
				new Lazy<Func<TTemp[], Task<TResult[]>>>(() => {
					var prepare = BuildPrepare<TTemp, TResult>(prepares());
					return async (tmps) =>
					{

						await Task.WhenAll(tmps.SelectMany(t => prepare(t, t.__Result)));
						return tmps.Select(t => t.__Result).ToArray();
					};
				}
				),
				new Lazy<IPagingQueryBuilder<TDataModel>>(()=> BuildPagingQueryBuilder<TDataModel>()),
				new Lazy<Expression<Func<IGrouping<int,TDataModel>,ISummaryWithCount>>>(()=>null)
				);
		}

		static QueryResultBuildHelper CreateQueryResultBuildHelper2<TDataModel, TResult>(
			Func<Expression> Expr,
			Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IValueConverter conv)>> prepares
			) 
		{
			return new QueryResultBuildHelper<TDataModel, TResult, TResult>(
					new Lazy<Expression<Func<TDataModel, TResult>>>(() => (Expression<Func<TDataModel, TResult>>)Expr()),
					new Lazy<Func<TResult[], Task<TResult[]>>>(() => {
						var prepare = BuildPrepare<TResult, TResult>(prepares());
						return async (tmps) =>
						{
							await Task.WhenAll(tmps.SelectMany(t => prepare(t, t)));
							return tmps;
						};
					}),
					new Lazy<IPagingQueryBuilder<TDataModel>>(() => BuildPagingQueryBuilder<TDataModel>()),
				new Lazy<Expression<Func<IGrouping<int, TDataModel>, ISummaryWithCount>>>(() => null)
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
						new Func<Expression>(()=>Expression.Lambda(
							Expression.MemberInit(
								Expression.New(TempType),
								TempBindings.Select(
									b => Expression.Bind(TempType.GetProperty(b.Item1), b.Item2)
									)
								.WithFirst(
									Expression.Bind(
										TempType.GetProperty("__Result"),
										Expression.MemberInit(
											Expression.New(DstType),
											DstBindings
										)
									)
								)
							),
							ArgSource
						)),
						new Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IValueConverter conv)>>(
							()=>Converters.Select(p => (prop:p.prop,propTemp: TempType.GetProperty(p.prop.Name), conv:p.conv))
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
						new Func<Expression>(
							()=>
								Expression.Lambda(
									Expression.MemberInit(
										Expression.New(DstType),
										DstBindings
									),
									ArgSource
								)
						),
						new Func<IEnumerable<(PropertyInfo prop, PropertyInfo tempProp, IValueConverter conv)>>(
							()=>Converters.Select(p => (prop:p.prop,propTemp: p.prop, conv:p.conv))
						)
					}
					);
			}
		}
	}
}
