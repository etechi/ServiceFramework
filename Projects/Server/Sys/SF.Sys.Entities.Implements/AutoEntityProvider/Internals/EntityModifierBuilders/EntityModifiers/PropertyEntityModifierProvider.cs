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
using System.Threading.Tasks;
using SF.Sys.Reflection;
using SF.Sys.Linq.Expressions;
using SF.Sys.Linq;

namespace SF.Sys.Entities.AutoEntityProvider.Internals.EntityModifiers
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
						tp =>$"数据实体{typeof(TDataModel)}属性{DataModelProperty.Name} 没有对应的模型实体{typeof(TEntity)}属性，但修改器{tp.GetType()}需要提供模型实体属性值"
						))
					.OrderBy(p => p.MergePriority)
					.ToArray();
			}

			static Type[] ModifierDefinitions { get; } = new[]
			{
				typeof(IAsyncEntityPropertyModifier<>),
				typeof(IAsyncEntityPropertyModifier<,>),
				typeof(IEntityPropertyModifier<>),
				typeof(IEntityPropertyModifier<,>),
				typeof(INoneEntityPropertyModifier)
			};
			static Type DetectModifierType(IEntityPropertyModifier Modifier)
			{
				var re = (from i in Modifier.GetType().AllInterfaces()
						  from t in ModifierDefinitions
						  where i==t || i.IsGeneric() && i.GetGenericTypeDefinition() == t
						  select i
						  ).ToArray();
				if (re.Length == 0)
					throw new NotSupportedException($"属性修改器{Modifier.GetType()}不支持执行函数");
				else if(re.Length>1)
					throw new NotSupportedException($"属性修改器{Modifier.GetType()}存在多个执行函数{re.Join(",")}");
				return re[0];
			}
			
			static async Task RunTasks(
				IEntityServiceContext ServiceContext, 
				IEntityModifyContext<TEntity, TDataModel> Context,
				Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>[] funcs
				)
			{
				Task lastTask = null;
				var Entity = Context.Editable;
				var Model = Context.Model;
				foreach (var f in funcs)
				{
					var t = f(ServiceContext, Context, Entity, Model, lastTask);
					if (t == null)
						break;
					await t;
					lastTask = t;
				}
			}
			static MethodInfo MethodRunTasks { get; } = typeof(EntityModifierCreator<TDataModel, TEntity>).GetMethod("RunTasks", BindingFlags.Static | BindingFlags.NonPublic);

			public Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, Task> TryBuild()
			{
				var modifiers =
					(from dp in typeof(TDataModel).AllPublicInstanceProperties()
					 let ep = typeof(TEntity).GetProperty(dp.Name)
					 from ms in FindPropModifiers(ep, dp)
					 group (DataProperty:dp, EntityProperty:ep, Modifier:ms) by dp into g
					 let m=g
						.OrderBy(i=>-i.Modifier.MergePriority)
						.Aggregate((l,h)=>(l.DataProperty,l.EntityProperty,Modifier:h.Modifier.Merge(l.Modifier)))
					orderby m.Modifier.ExecutePriority
					select m
					).ToArray();

				var ArgManager = Expression.Parameter(typeof(IEntityServiceContext));
				var ArgModifierContext = Expression.Parameter(typeof(IEntityModifyContext<TEntity, TDataModel>));
				var ArgTask = Expression.Parameter(typeof(Task));
				var ArgModel = Expression.Parameter(typeof(TDataModel));
				var ArgEntity = Expression.Parameter(typeof(TEntity));

				var UpdateGroups = new List<(PropertyInfo dataProperty, Expression[] exprs)>();
				var Updates = new List<Expression>();

				foreach (var m in modifiers)
				{
					var modifierType = DetectModifierType(m.Modifier);
					if (modifierType == typeof(INoneEntityPropertyModifier))
						continue;
					var modifierGenericType = modifierType.GetGenericTypeDefinition();
					var expr = modifierType.GenericTypeArguments.Length == 1?
						Expression.Call(
							Expression.Constant(m.Modifier, modifierType),
							modifierType.GetMethod("Execute"),
							ArgManager,
							ArgModifierContext,
							ArgModel.GetMember(m.DataProperty)
							) :
						Expression.Call(
							Expression.Constant(m.Modifier, modifierType),
							modifierType.GetMethod("Execute"),
							ArgManager,
							ArgModifierContext,
							ArgModel.GetMember(m.DataProperty),
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
				var funcs = new List<Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>>();
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
									.GetMember(nameof(Task<int>.Result))
								)
							);
					exprs.AddRange(g.exprs);
					lastDataProperty = g.dataProperty;
					if (lastDataProperty == null)
						exprs.Add(Expression.Constant(null, typeof(Task)));

					funcs.Add(
						Expression.Block(exprs).Compile<
							Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, TEntity, TDataModel, Task, Task>
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
					).Compile<Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, Task>>(
					ArgManager,
					ArgModifierContext
					);
				
			}
		}

		class Modifier<TEntity, TDataModel> : IEntityModifier<TEntity, TDataModel>
			where TDataModel : class
		{
			Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, Task> Executor { get; }
			public Modifier(Func<IEntityServiceContext, IEntityModifyContext<TEntity, TDataModel>, Task> Executor)
			{
				this.Executor = Executor;
			}
			public int Priority =>100;
			public Task Execute(IEntityServiceContext Manager, IEntityModifyContext<TEntity, TDataModel> Context)
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
