using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Entities
{
	

    public static class EntityExtension
	{
		public static IObjectEntity Create(this IObjectEntity model,DateTime time)
		{
			model.CreatedTime = time;
			return model;
		}
		public static IObjectEntity Update(this IObjectEntity model,IObjectEntity entity,DateTime time)
		{
			model.UpdatedTime = time;
			model.Name = entity.Name;
			model.LogicState = entity.LogicState;
			return model;
		}

		public static IUIObjectEntity Update(this IUIObjectEntity model, IUIObjectEntity entity, DateTime time) 
		{
			((IObjectEntity)model).Update(entity, time);
			model.Title = entity.Title;
			model.SubTitle = entity.SubTitle;
			model.Remarks = entity.Remarks;
			model.Description = entity.Description;
			model.Icon = entity.Icon;
			model.Image = entity.Image;
			return model;

		}


		static string[] GetTypePropertyNames(Type type) =>
			type.GetInterfaceMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
			.Where(m => m is PropertyInfo)
			.Select(p => p.Name)
			.ToArray();

		static string[] ObjectEntityPropertyNames { get; } = GetTypePropertyNames(typeof(IObjectEntity));
		static string[] UIObjectEntityPropertyNames { get; } = GetTypePropertyNames(typeof(IUIObjectEntity));
		static string[] EventEntityPropertyNames { get; } = GetTypePropertyNames(typeof(IEventEntity));

		public static IContextQueryable<TResult> SelectWithProperties<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, string[] Properties)
		{
			var expr = selector.Body as MemberInitExpression;
			if (expr != null)
			{
				var ms = expr.Bindings.ToDictionary(b => b.Member.Name);
				var arg = selector.Parameters[0];
				var binds = expr.Bindings.Concat(
					Properties
					.Where(p => !ms.ContainsKey(p))
					.Select(p =>
						Expression.Bind(
							typeof(TResult).GetProperty(p, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty),
							Expression.Property(arg, typeof(TSource).GetProperty(p, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
						)
					)
				);
				selector = Expression.Lambda<Func<TSource, TResult>>(
					Expression.MemberInit(
						expr.NewExpression,
						binds
						),
					arg
					);
			}

			return source.New(source.Queryable.Select(selector));
		}

		static Expression<Func<TSource, TResult>> Lambda<TSource, TResult>(Expression<Func<TSource, TResult>> expr)
			=> expr;

		
		public static Expression<Func<TSource, TResult>> MergeMemberInit<TSource, TResult>(
			this Expression<Func<TSource, TResult>> orgMapper,
			Expression<Func<TSource, TResult>> newMapper
			)
		{
			var oExpr = orgMapper.Body as MemberInitExpression;
			if (oExpr == null)
				throw new NotSupportedException();

			var nExpr = newMapper.Body as MemberInitExpression;
			if (nExpr == null)
				throw new NotSupportedException();

			var orgArg = orgMapper.Parameters[0];
			var newArg = newMapper.Parameters[0];
			var argMap = new Dictionary<ParameterExpression, ParameterExpression> { { orgArg, newArg } };

			var binds = nExpr.Bindings.ToDictionary(b => b.Member.Name);
			oExpr.Bindings
				.Where(b => !binds.ContainsKey(b.Member.Name))
				.ForEach(b => 
					binds[b.Member.Name] = b.ReplaceArguments(argMap)
					);

			return Expression.Lambda<Func<TSource, TResult>>(
					Expression.MemberInit(
						nExpr.NewExpression,
						binds.Values
						),
					newArg
					);
		}

		public static Expression<Func<TSource, TResult>> GetObjectEntityMapper<TSource, TResult>()
			where TSource : IObjectEntity
			where TResult : IObjectEntity, new()
			=> Lambda<TSource, TResult>(
				s => new TResult
				{
					Name = s.Name,
					LogicState = s.LogicState,
					CreatedTime = s.CreatedTime,
					UpdatedTime = s.UpdatedTime
				}
				);

		static Expression<Func<TSource, TResult>> GetUIObjectEntityMapper<TSource, TResult>(this IContextQueryable<TSource> source)
			where TSource : IUIObjectEntity
			where TResult : IUIObjectEntity, new()
			=> 
				GetObjectEntityMapper<TSource, TResult>()
					.MergeMemberInit(
						s => new TResult
						{
							Title=s.Title,
							SubTitle = s.SubTitle,
							Remarks = s.Remarks,
							Description = s.Description,
							Image = s.Image,
							Icon =s.Icon,
						}
					);

		static Expression<Func<TSource, TResult>> GetContainerItemEntityMapper<TSource, TResult>(this IContextQueryable<TSource> source)
			where TSource : IItemEntity
			where TResult : IItemEntity, new()
			=> Lambda<TSource, TResult>(
				s => new TResult
				{
					ContainerId=s.ContainerId
				}
			);

		public static IContextQueryable<TResult> SelectObjectEntity<TSource, TResult>(
			this IContextQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector
			)
			where TSource : IObjectEntity
			where TResult : IObjectEntity, new()
		{
			return source.SelectWithProperties(
				selector,
				ObjectEntityPropertyNames
				);
		}
		public static IContextQueryable<TResult> SelectUIObjectEntity<TSource, TResult>(
			this IContextQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector
			)
			where TResult : IUIObjectEntity, new()
			where TSource : IUIObjectEntity
		{
			return source.SelectWithProperties(
				selector,
				UIObjectEntityPropertyNames
				);
		}
		public static IContextQueryable<TResult> SelectEventEntity<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return source.SelectWithProperties(selector, EventEntityPropertyNames);
		}
	}
}
