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


		//static class TypePropertyNames<T>
		//{
		//	public static string[] Values{ get; } = GetTypePropertyNames(typeof(T));

		//}

		//public static IContextQueryable<TResult> SelectWithProperties<TSource, TResult>(this IContextQueryable<TSource> source, Expression<Func<TSource, TResult>> selector, string[] Properties)
		//{
		//	var expr = selector.Body as MemberInitExpression;
		//	if (expr != null)
		//	{
		//		var ms = expr.Bindings.ToDictionary(b => b.Member.Name);
		//		var arg = selector.Parameters[0];
		//		var binds = expr.Bindings.Concat(
		//			Properties
		//			.Where(p => !ms.ContainsKey(p))
		//			.Select(p =>
		//				Expression.Bind(
		//					typeof(TResult).GetProperty(p, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty),
		//					Expression.Property(arg, typeof(TSource).GetProperty(p, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
		//				)
		//			)
		//		);
		//		selector = Expression.Lambda<Func<TSource, TResult>>(
		//			Expression.MemberInit(
		//				expr.NewExpression,
		//				binds
		//				),
		//			arg
		//			);
		//	}

		//	return source.New(source.Queryable.Select(selector));
		//}

		//static Expression<Func<TSource, TResult>> Lambda<TSource, TResult>(Expression<Func<TSource, TResult>> expr)
		//	=> expr;


		static void AddBinds(
			Dictionary<string, MemberBinding> dict,
			IEnumerable<MemberBinding> binds,
			Dictionary<Expression, Expression> argMap
			)
		{
			foreach (var b in binds)
			{
				if (dict.ContainsKey(b.Member.Name))
					continue;

				dict[b.Member.Name] = b.Replace(argMap);
			}
		}

		public static Expression<Func<TSource, TResult>> MergeMemberInit<TSource, TResult>(
			this Expression<Func<TSource, TResult>> orgMapper,
			params Expression<Func<TSource, TResult>>[] newMappers
			)
		{
			if (newMappers == null || newMappers.Length == 0)
				return orgMapper;

			var lastMapper = newMappers[newMappers.Length - 1].Body as MemberInitExpression;
			if (lastMapper == null)
				throw new NotSupportedException();
			var dstArg = newMappers[newMappers.Length - 1].Parameters[0];


			var binds = lastMapper.Bindings.ToDictionary(b => b.Member.Name);

			for(var i = newMappers.Length - 2; i >= 0; i--)
			{
				var mapper = newMappers[i].Body as MemberInitExpression;
				if (mapper == null)
					throw new NotSupportedException();

				AddBinds(
					binds,
					mapper.Bindings,
					new Dictionary<Expression, Expression> { { newMappers[i].Parameters[0], dstArg } }
					);
			}
			var firstMapper = orgMapper.Body as MemberInitExpression;
			if (firstMapper == null)
				throw new NotSupportedException();
			AddBinds(
				binds,
				firstMapper.Bindings,
				new Dictionary<Expression, Expression> { { orgMapper.Parameters[0], dstArg } }
				);


			return Expression.Lambda<Func<TSource, TResult>>(
					Expression.MemberInit(
						lastMapper.NewExpression,
						binds.Values
						),
					dstArg
					);
		}

		//public static Expression<Func<TSource, TResult>> GetObjectEntityMapper<TSource, TResult>()
		//	where TSource : IObjectEntity
		//	where TResult : IObjectEntity, new()
		//	=> Lambda<TSource, TResult>(
		//		s => new TResult
		//		{
		//			Name = s.Name,
		//			LogicState = s.LogicState,
		//			CreatedTime = s.CreatedTime,
		//			UpdatedTime = s.UpdatedTime
		//		}
		//		);

		//static Expression<Func<TSource, TResult>> GetUIObjectEntityMapper<TSource, TResult>(this IContextQueryable<TSource> source)
		//	where TSource : IUIObjectEntity
		//	where TResult : IUIObjectEntity, new()
		//	=> 
		//		GetObjectEntityMapper<TSource, TResult>()
		//			.MergeMemberInit(
		//				s => new TResult
		//				{
		//					Title=s.Title,
		//					SubTitle = s.SubTitle,
		//					Remarks = s.Remarks,
		//					Description = s.Description,
		//					Image = s.Image,
		//					Icon =s.Icon,
		//				}
		//			);

		//static Expression<Func<TSource, TResult>> GetContainerItemEntityMapper<TSource, TResult>(this IContextQueryable<TSource> source)
		//	where TSource : IItemEntity
		//	where TResult : IItemEntity, new()
		//	=> Lambda<TSource, TResult>(
		//		s => new TResult
		//		{
		//			ContainerId=s.ContainerId
		//		}
		//	);
		public static IContextQueryable<TResult> SelectObjectEntity<TSource, TResult>(
			this IContextQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector
			)
			where TSource : IObjectEntity
			where TResult : IObjectEntity, new()
		{
			return source.Select(
				MergeMemberInit<TSource,TResult>(
				s=>new TResult
				{
					Name=s.Name,
					LogicState=s.LogicState,
					CreatedTime=s.CreatedTime,
					UpdatedTime=s.UpdatedTime
				},
				selector
				));
		}
		public static IContextQueryable<TResult> SelectUIObjectEntity<TSource, TResult>(
			this IContextQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector
			)
			where TSource : IUIObjectEntity
			where TResult : IUIObjectEntity, new()
		{
			return source.Select(
				MergeMemberInit<TSource, TResult>(
				s => new TResult
				{
					Name = s.Name,
					LogicState = s.LogicState,
					CreatedTime = s.CreatedTime,
					UpdatedTime = s.UpdatedTime,
					Title = s.Title,
					SubTitle = s.SubTitle,
					Remarks = s.Remarks,
					Description = s.Description,
					Image = s.Image,
					Icon=s.Icon
				},
				selector
				));
		}
		public static IContextQueryable<TResult> SelectEventEntity<TSource, TResult>(
			this IContextQueryable<TSource> source,
			Expression<Func<TSource, TResult>> selector
			)
			where TSource : IEventEntity
			where TResult : IEventEntity, new()
		{
			return source.Select(
				MergeMemberInit<TSource, TResult>(
				s => new TResult
				{
					UserId=s.UserId,
					Time=s.Time
				},
				selector
				));
		}
		//public static IContextQueryable<TResult> SelectObjectEntity<TSource, TResult>(
		//	this IContextQueryable<TSource> source,
		//	Expression<Func<TSource, TResult>> selector
		//	)
		//	where TSource : IObjectEntity
		//	where TResult : IObjectEntity, new()
		//{
		//	return source.SelectWithProperties(
		//		selector,
		//		TypePropertyNames<IObjectEntity>.Values
		//		);
		//}
		//public static IContextQueryable<TResult> SelectUIObjectEntity<TSource, TResult>(
		//	this IContextQueryable<TSource> source,
		//	Expression<Func<TSource, TResult>> selector
		//	)
		//	where TResult : IUIObjectEntity, new()
		//	where TSource : IUIObjectEntity
		//{
		//	return source.SelectWithProperties(
		//		selector,
		//		TypePropertyNames<IUIObjectEntity>.Values
		//		);
		//}
		//public static IContextQueryable<TResult> SelectEventEntity<TSource, TResult>(
		//	this IContextQueryable<TSource> source, 
		//	Expression<Func<TSource, TResult>> selector
		//	)
		//{
		//	return source.SelectWithProperties(
		//		selector, 
		//		TypePropertyNames<IEventEntity>.Values
		//		);
		//}
	}
}
