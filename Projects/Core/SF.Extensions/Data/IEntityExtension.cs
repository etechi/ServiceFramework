using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
namespace SF.Data
{
	

    public static class IEntityExtension
	{
		public static IEntity Create(this IEntity model,DateTime time)
		{
			model.CreatedTime = time;
			return model;
		}
		public static IEntity Update(this IEntity model,IEntity entity,DateTime time)
		{
			model.UpdatedTime = time;
			model.Name = entity.Name;
			model.ObjectState = entity.ObjectState;
			return model;
		}

		public static IUIEntity Update(this IUIEntity model, IUIEntity entity, DateTime time) 
		{
			((IEntity)model).Update(entity, time);
			model.Title = entity.Title;
			model.SubTitle = entity.SubTitle;
			model.Remarks = entity.Remarks;
			model.Description = entity.Description;
			model.Icon = entity.Icon;
			model.Image = entity.Image;
			return model;

		}


		static string[] GetTypePropertyNames(Type type)=>
			type.GetInterfaceMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty)
			.Where(m=>m is PropertyInfo)
			.Select(p => p.Name)
			.ToArray();

		static string[] EntityPropertyNames { get; } = GetTypePropertyNames(typeof(IEntity));
		static string[] UIEntityPropertyNames { get; } = GetTypePropertyNames(typeof(IUIEntity));

		public static IContextQueryable<TResult> SelectWithProperties<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector,string[] Properties)
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
		public static IContextQueryable<TResult> SelectEntity<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return source.SelectWithProperties(selector, EntityPropertyNames);
		}
		public static IContextQueryable<TResult> SelectUIEntity<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
		{
			return source.SelectWithProperties(selector, UIEntityPropertyNames);
		}

	}
}
