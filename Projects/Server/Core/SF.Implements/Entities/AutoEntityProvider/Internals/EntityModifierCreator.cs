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

	public abstract class EntityModifier
	{

	}
	public class EntityModifier<TSource, TTarget> :
		EntityModifier,
		IEntityModifier<TSource, TTarget>
	{
		public Lazy<Func<TSource, TTarget, Task>> Modifier { get; }
		public EntityModifier(Lazy<Func<TSource, TTarget, Task>> Modifier)
		{
			this.Modifier = Modifier;
		}
		Func<TSource, TTarget, Task> IEntityModifier<TSource, TTarget>.Modifier => Modifier.Value;
	}

	public class EntityModifierCreator
	{
		Type DataModelType { get; }
		Type EntityType { get; }
		NamedServiceResolver<IEntityModifierValueConverter> EntityModifierValueConverter { get; }
		ParameterExpression ArgModel { get; }
		ParameterExpression ArgEntity { get; }
		string ConvertMode { get; }
		List<(PropertyInfo prop, PropertyInfo srcProp, IEntityModifierValueConverter conv)> Converters { get; } = new List<(PropertyInfo prop, PropertyInfo srcProp,IEntityModifierValueConverter conv)>();
		List<Expression> Assigns { get; } = new List<Expression>();

		public EntityModifierCreator(
			Type DataModelType, 
			Type EntityType, 
			NamedServiceResolver<IEntityModifierValueConverter> EntityModifierValueConverter,
			string ConvertMode
			)
		{
			this.ConvertMode = ConvertMode;
			this.DataModelType = DataModelType;
			this.EntityType = EntityType;
			this.EntityModifierValueConverter = EntityModifierValueConverter;
			this.ArgModel = Expression.Parameter(DataModelType);
			this.ArgEntity = Expression.Parameter(EntityType);
		}

		IEntityModifierValueConverter FindValueConverter(PropertyInfo srcProp, PropertyInfo dstProp)
		{
			if (EntityModifierValueConverter == null)
				return null;

			IEntityModifierValueConverter re = null;

			if (srcProp != null && null != (re = EntityModifierValueConverter(
				$"{ConvertMode}/{srcProp.DeclaringType.FullName}:{srcProp.Name}->{dstProp.DeclaringType.FullName}:{dstProp.Name}"
				)))
				return re;

			if (srcProp != null)
			{
				var ivo = srcProp.GetCustomAttribute<IsValueOfAttribute>();
				if (ivo != null && null != (re = EntityModifierValueConverter($"{ConvertMode}/{ivo.Type.FullName}")))
				{
					var ivoDataType = ivo.ValueType;
					if (!ivoDataType.CanSimpleConvertTo(dstProp.PropertyType))
						throw new NotSupportedException($"实体类型{srcProp.DeclaringType}属性{srcProp.Name}细分类型{ivo.Type}的原始类型{ivoDataType}和数据实体{dstProp.DeclaringType}属性{dstProp.Name}数据类型{dstProp.PropertyType}不兼容");
					return re;
				}
			}

			if (srcProp!=null && null != (re = EntityModifierValueConverter($"{ConvertMode}/{srcProp.PropertyType.FullName}->{dstProp.PropertyType.FullName}")))
				return re;

			if (srcProp != null && null != (re = EntityModifierValueConverter($"{ConvertMode}/{dstProp.PropertyType.FullName}")))
				return re;

			return null;
		}

		void BuildPropSetter(PropertyInfo dstProp)
		{
			var srcProp = DataModelType.GetProperty(dstProp.Name, BindingFlags.Instance | BindingFlags.Public);
			var vc = FindValueConverter(srcProp, dstProp);
			if (vc != null)
			{
				Converters.Add((dstProp, srcProp, vc));
			}
			else if (srcProp == null)
				return;
			else
			{
				var src= (Expression)Expression.Property(ArgEntity, srcProp);
				if(srcProp.PropertyType!=dstProp.PropertyType)
				{
					if (srcProp.PropertyType.CanSimpleConvertTo(dstProp.PropertyType))
						src = Expression.Convert(src, dstProp.PropertyType);
					else
						throw new NotSupportedException($"来源字段{EntityType.FullName}.{srcProp.Name} 的类型{srcProp.PropertyType} 和目标字段 {DataModelType.FullName}.{dstProp.Name} 的类型不兼容");
				}
				Assigns.Add(
					Expression.Call(
						ArgModel,
						dstProp.GetSetMethod(),
						src
					)
				);
			}
		}

		class EmptyAssign<TDataModel, TResult>{
			public static Func<TDataModel, TResult, Task[]> AsyncAssign { get; } = (m, e) => Array.Empty<Task>();
			public static Action<TDataModel, TResult> SyncAssign { get; } = (m, e) => { };
		}
		
		static EntityModifier CreateModifier<TDataModel, TResult>(
			Func<(object,object)> Assign
			)
		{
			return new EntityModifier<TDataModel, TResult>(
				new Lazy<Func<TDataModel, TResult, Task>>(
					()=>
					{
						var re = Assign();
						var sa = (Action<TDataModel, TResult>)re.Item1 ?? EmptyAssign<TDataModel,TResult>.SyncAssign;
						var aa = ((Func<TDataModel, TResult, Task[]>)re.Item2)?? EmptyAssign<TDataModel, TResult>.AsyncAssign;
						return async (m, e) =>
						{
							sa(m, e);
							await Task.WhenAll(aa(m, e));
						};
					})
				);
		}
		static MethodInfo MethodCreateModifier { get; } = typeof(EntityModifierCreator).GetMethodExt(
			"CreateModifier",
			typeof(Expression),
			typeof(IEnumerable<(PropertyInfo prop, PropertyInfo propTemp, IValueConverter conv)>)
			);
		public EntityModifier Build()
		{
			return (EntityModifier)MethodCreateModifier.MakeGenericMethod(DataModelType, EntityType).Invoke(
				null,
				new object[]{
					new Func<(object,object)>(() =>
					{
						foreach (var prop in DataModelType.AllPublicInstanceProperties())
							BuildPropSetter(prop);
						var SyncAssign=Assigns.Count==0?null:
							Expression.Lambda(
								Expression.Block(
									Assigns
								),
								ArgModel,
								ArgEntity
							).Compile();

						var AsyncAssign=Converters.Count==0?null:
							Expression.Lambda(
								Expression.NewArrayInit(
								typeof(Task),
								Converters.Select(p =>
								{
									var convType = p.conv.GetType();
									var convTypeArgs=convType.GetGenericArguments();

									var converterCall = Expression.Call(
										Expression.Convert(
											Expression.Constant(p.conv),
											convType
											),
										convType.GetMethod("Convert", BindingFlags.Public | BindingFlags.Instance),
										ArgEntity,
										p.srcProp == null ?
										(Expression)Expression.Constant(
											convTypeArgs[0].GetDefaultValue(),
											convTypeArgs[0]
											) :
										(Expression)Expression.Property(ArgEntity, p.srcProp),
										Expression.Constant(p.srcProp!=null)
									);
									var taskType = typeof(Task<>).MakeGenericType(p.prop.PropertyType);
									var argTask = Expression.Parameter(typeof(Task<>).MakeGenericType(p.prop.PropertyType));
									var argContext = Expression.Parameter(typeof(object));
									var bind = Expression.Lambda(
										Expression.Call(
											Expression.Convert(argContext, DataModelType),
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
											ArgModel
											),
										typeof(Task)
										);
								}
							)), ArgModel, ArgEntity).Compile();

						return (SyncAssign,AsyncAssign);
					})
				}
			);
		}
	}
}
