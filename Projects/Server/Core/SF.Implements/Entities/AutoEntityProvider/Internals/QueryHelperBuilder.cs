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
	public abstract class QueryHelper
	{
		public abstract Type TempType { get; }
		public abstract Type DateModelType { get; }
	}

	public class QueryHelper<TDataModel,TTemp,TResult>:
		QueryHelper
	{
		public QueryHelper(
			Expression<Func<TDataModel, TTemp>> FuncMapModelToTemp,
			Func<TTemp[], Task<TResult[]>> FuncMapTempToDetail
			)
		{
			this.FuncMapModelToTemp = FuncMapModelToTemp;
			this.FuncMapTempToDetail = FuncMapTempToDetail;
		}
		public Expression<Func<TDataModel, TTemp>> FuncMapModelToTemp { get; }
		public Func<TTemp[], Task<TResult[]>> FuncMapTempToDetail { get; }

		public override Type TempType => typeof(TTemp);

		public override Type DateModelType => typeof(TDataModel);
	}
	public class QueryHelper<TDataModel, TResult> :
		QueryHelper<TDataModel, TResult, TResult>
	{
		public QueryHelper(
			Expression<Func<TDataModel, TResult>> FuncMapModelToTemp, 
			Func<TResult[], Task<TResult[]>> FuncMapTempToDetail) : 
			base(FuncMapModelToTemp, FuncMapTempToDetail)
		{
		}
	}
	class BaseTempModel<TResult> 
	{
		public TResult _Result { get; set; }
	}
	public class QueryHelperBuilder
	{
		Type SrcType { get; }
		Type DstType { get; }
		ParameterExpression ArgSource{ get; }
		List<MemberBinding> DstBindings { get; } = new List<MemberBinding>();
		List<(string,Expression)> TempBindings { get; } = new List<(string, Expression)>();
		TypeBuilder TempTypeBuilder;
		Type TempType;

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
		public QueryHelperBuilder(Type SrcType,Type DstType)
		{
			this.SrcType = SrcType;
			this.DstType = DstType;
		}
		bool isNumberType(Type type)
		{
			switch (type.GetTypeCode())
			{
				case TypeCode.Boolean:
				case TypeCode.Byte:
				case TypeCode.Char:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.SByte:
				case TypeCode.Single:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return true;
			}
			if(type.IsEnumType())
				return true;
			return false;
		}
		bool IsTypeCompatible(Type src,Type dst)
		{
			if (src == dst) return true;
			if (dst.IsAssignableFrom(src)) return true;
			var srcIsNumber = isNumberType(src);
			var dstIsNumber = isNumberType(dst);
			return srcIsNumber == dstIsNumber;
		}
		public interface IValueConverter
		{
			Type TempFieldType { get; }
			Expression SourceToTemp(Expression src, PropertyInfo srcProp);
		}
		public interface IValueConverter<T,D>: IValueConverter
		{
			Task<D> TempToDest(object src, T value);
		}
		IValueConverter FindValueConverter(PropertyInfo srcProp,PropertyInfo dstProp)
		{
			IValueConverter<int, int> c = null;
			c.TempToDest(null, 1).ContinueWith(
				(t) => { var a = t.Result; }
				);
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

			}
			else if (srcProp == null)
				return;
			else
			{
				var src = (Expression)Expression.Property(ArgSource, srcProp);
 				if (dstProp.PropertyType != srcProp.PropertyType)
				{
					if (IsTypeCompatible(srcProp.PropertyType, dstProp.PropertyType))
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
		class Creator<TDataModel, TTemp, TResult>
			where TTemp:BaseTempModel<TResult> 
		{
			
			public static Func<TTemp[],Task<TResult[]>> TempToResult(Func<TTemp,TResult,Task[]> Prepare)
			{
				return async (tmps) =>
				{
					await Task.WhenAll(tmps.SelectMany(t => Prepare(t, t._Result)));
					return tmps.Select(t => t._Result).ToArray();
				};
			}
			public static QueryHelper Build(
				Expression<Func<TDataModel,TTemp>> Expr,
				IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)> prepares
				)
			{
				return new QueryHelper<TDataModel, TTemp, TResult>(
					Expr,
					TempToResult(BuildPrepare<TTemp,TResult>(prepares))
					);
			}
		}
		class Creator<TDataModel, TResult>
		{
			public static Func<TResult[], Task<TResult[]>> TempToResult(Func<TResult, TResult, Task[]> Prepare)
			{
				return async (tmps) =>
				{
					await Task.WhenAll(tmps.SelectMany(t => Prepare(t, t)));
					return tmps;
				};
			}
			public static QueryHelper Build(
				Expression<Func<TDataModel, TResult>> Expr,
				IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)> prepares
				)
			{
				return new QueryHelper<TDataModel, TResult>(
					Expr,
					TempToResult(BuildPrepare<TResult, TResult>(prepares))
					);
			}
		}
		public QueryHelper Build()
		{
			foreach(var prop in DstType.AllPublicInstanceProperties())
				BuildBindings(prop);

			if (TempTypeBuilder != null)
			{
				TempType = TempTypeBuilder.CreateTypeInfo().AsType();

				return (QueryHelper)Activator.CreateInstance(
					typeof(QueryHelper<,,>).MakeGenericType(SrcType, TempType, DstType),


					);

			}
			else
			{
				return (QueryHelper)Activator.CreateInstance(
					typeof(QueryHelper<,>).MakeGenericType(SrcType, DstType),


					);
			}
		}
	}
}
