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

namespace SF.Entities.AutoEntityProvider.Internals.EntityModifiers
{
	public class PropertyEntityModifierProvider : IEntityModifierProvider
	{
		public PropertyEntityModifierProvider()
		{
		}

		public class EntityModifierCreator<TDataModel, TEntity>
			where TDataModel:class
		{
			IEnumerable<IEntityPropertyModifierProvider> PropertyModifierProviders { get; }
			DataActionType ActionType { get; }
			List<(PropertyInfo prop, PropertyInfo srcProp, IEntityPropertyModifier conv)> Converters { get; } =
				new List<(PropertyInfo prop, PropertyInfo srcProp, IEntityPropertyModifier conv)>();

			List<Expression> Assigns { get; } = new List<Expression>();

			public EntityModifierCreator(
				IEnumerable<IEntityPropertyModifierProvider> PropertyModifierProviders,
				DataActionType ActionType
				)
			{
				this.ActionType = ActionType;
				this.PropertyModifierProviders = PropertyModifierProviders;
			}

			IEntityPropertyModifier[] FindPropModifiers(PropertyInfo EntityProperty, PropertyInfo DataModelProperty)
			{
				return PropertyModifierProviders
					.Select(p =>
						p.GetPropertyModifier(ActionType, typeof(TEntity), EntityProperty, typeof(TDataModel), DataModelProperty)
					).Where(p => p != null)
					.Select(p=>p.Assert(
						tp=>!(tp.GetType().GenericTypeArguments.Length==2 && EntityProperty==null),
						tp =>$"����ʵ��{typeof(TDataModel)}����{DataModelProperty.Name} û�ж�Ӧ��ģ��ʵ��{typeof(TEntity)}���ԣ����޸���{tp.GetType()}��Ҫ�ṩģ��ʵ������ֵ"
						))
					.OrderBy(p => p.Priority)
					.ToArray();
			}

			static Type[] ModifierDefinitions { get; } = new[]
			{
				typeof(IAsyncEntityPropertyModifier<>),
				typeof(IAsyncEntityPropertyModifier<,>),
				typeof(IEntityPropertyModifier<>),
				typeof(IEntityPropertyModifier<,>)
			};
			static Type DetectModifierType(IEntityPropertyModifier Modifier)
			{
				var re = (from i in Modifier.GetType().AllInterfaces()
						  from t in ModifierDefinitions
						  where i.IsGeneric() && i.GetGenericTypeDefinition() == t
						  select i).ToArray();
				if (re.Length == 0)
					throw new NotSupportedException($"�����޸���{Modifier.GetType()}��֧��ִ�к���");
				else if(re.Length>1)
					throw new NotSupportedException($"�����޸���{Modifier.GetType()}���ڶ��ִ�к���{re.Join(",")}");
				return re[0];
			}
			
			static async Task RunTasks(
				IDataSetEntityManager<TEntity, TDataModel> Manager, 
				IEntityModifyContext<TEntity, TDataModel> Context,
				Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>[] funcs
				)
			{
				Task lastTask = null;
				var Entity = Context.Editable;
				var Model = Context.Model;
				foreach (var f in funcs)
				{
					var t = f(Manager, Context, Entity, Model, lastTask);
					if (t == null)
						break;
					await t;
					lastTask = t;
				}
			}
			static MethodInfo MethodRunTasks = typeof(EntityModifierCreator<TDataModel, TEntity>).GetMethod("RunTasks",BindingFlags.Static| BindingFlags.NonPublic);

			public Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, Task> TryBuild()
			{
				var modifiers =
					(from dp in typeof(TDataModel).AllPublicInstanceProperties()
					 let ep = typeof(TEntity).GetProperty(dp.Name)
					 from ms in FindPropModifiers(ep, dp)
					 select (DataProperty:dp, EntityProperty:ep, Modifier:ms)
					).ToArray();

				var ArgManager = Expression.Parameter(typeof(IDataSetEntityManager<TEntity, TDataModel>));
				var ArgModifierContext = Expression.Parameter(typeof(IEntityModifyContext<TEntity, TDataModel>));
				var ArgTask = Expression.Parameter(typeof(Task));
				var ArgModel = Expression.Parameter(typeof(TDataModel));
				var ArgEntity = Expression.Parameter(typeof(TEntity));

				var UpdateGroups = new List<(PropertyInfo dataProperty, Expression[] exprs)>();
				var Updates = new List<Expression>();

				foreach (var m in modifiers)
				{
					var modifierType = DetectModifierType(m.Modifier);
					var modifierGenericType = modifierType.GetGenericTypeDefinition();
					var expr = m.EntityProperty == null ?
						Expression.Call(
							Expression.Constant(m.Modifier, modifierType),
							modifierType.GetMethod("Execute"),
							ArgManager,
							ArgModifierContext
							) :
						Expression.Call(
							Expression.Constant(m.Modifier, modifierType),
							modifierType.GetMethod("Execute"),
							ArgManager,
							ArgModifierContext,
							ArgEntity.GetMember(m.EntityProperty)
							);
					if (typeof(Task).IsAssignableFrom(expr.Type))
					{
						Updates.Add(expr);
						UpdateGroups.Add((m.DataProperty, Updates.ToArray()));
						Updates.Clear();
					}
					else
					{
						Updates.Add(ArgModel.SetProperty(m.DataProperty, expr));
					}
				}
				UpdateGroups.Add((null, Updates.ToArray()));

				PropertyInfo lastDataProperty = null;
				var funcs = new List<Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>>();
				var exprs = new List<Expression>(); 
				foreach (var g in UpdateGroups)
				{
					exprs.Clear();
					if (lastDataProperty != null)
						exprs.Add(
							ArgModel.SetProperty(
								lastDataProperty,
								ArgTask
									.To(typeof(Task<>).MakeGenericType(lastDataProperty.PropertyType))
									.GetMember("Value")
								)
							);
					exprs.AddRange(g.exprs);
					lastDataProperty = g.dataProperty;
					if (lastDataProperty == null)
						exprs.Add(Expression.Constant(null, typeof(Task)));

					funcs.Add(
						Expression.Block(exprs).Compile<
							Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>
						>(
							ArgManager,
							ArgModifierContext,
							ArgEntity,
							ArgModel,
							ArgTask
						)
						);
				}

				return Expression.Call(
					null,
					MethodRunTasks,
					ArgManager,
					ArgModifierContext,
					Expression.Constant(funcs.ToArray())
					).Compile<Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, Task>>(
					ArgManager,
					ArgModifierContext
					);
				
			}
		}

		class Modifier<TEntity, TDataModel> : IEntityModifier<TEntity, TDataModel>
			where TDataModel : class
		{
			Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, Task> Executor { get; }
			public Modifier(Func<IDataSetEntityManager<TEntity, TDataModel>, IEntityModifyContext<TEntity, TDataModel>, Task> Executor)
			{
				this.Executor = Executor;
			}
			public int Priority => 0;
			public Task Execute(IDataSetEntityManager<TEntity, TDataModel> Manager, IEntityModifyContext<TEntity, TDataModel> Context)
			{
				return Executor(Manager, Context);
			}
		}

		IEnumerable<IEntityPropertyModifierProvider> EntityPropertyModifierProviders { get; }

		public PropertyEntityModifierProvider(IEnumerable<IEntityPropertyModifierProvider> EntityPropertyModifierProviders)
		{
			this.EntityPropertyModifierProviders = EntityPropertyModifierProviders;
		}

		public IEntityModifier<TEntity, TDataModel> GetEntityModifier<TEntity, TDataModel>(DataActionType ActionType)
			where TDataModel : class
		{
			var emc = new EntityModifierCreator<TDataModel, TEntity>(
				EntityPropertyModifierProviders,
				ActionType
				);
			var func = emc.TryBuild();
			if (func == null)
				return null;
			return new Modifier<TEntity, TDataModel>(func);
		}
	}
}