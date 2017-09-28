using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace SF.Entities
{
	public interface IPagingQueryBuilderInitializer<T>
	{
		IPagingQueryBuilderInitializer<T> Add(string name, Func<IContextQueryable<T>, bool, IContextQueryable<T>> expr, bool defaultOrderDesc = false);
	}
	public static class IPagingQueryBuilderInitializerExtension
	{
		public static IPagingQueryBuilderInitializer<T> Add<T,K>(
			this IPagingQueryBuilderInitializer<T> initializer, 
			string name, 
			Expression<Func<T,K>> expr, 
			bool defaultOrderDesc = false
			)
		{
			return initializer.Add(name, (q, desc) => desc ? q.OrderByDescending(expr) : q.OrderBy(expr), defaultOrderDesc);
		}
	}
	public class PagingQueryBuilder<T>: IPagingQueryBuilder<T>
	{
		class SortField
		{
			public bool defaultOrderDesc;
			public Func<IContextQueryable<T>, bool, IContextQueryable<T>> query;
        }
		
		string DefaultSortMethod { get; }
		Dictionary<string, SortField> Fields { get; } = new Dictionary<string, SortField>();
		class PagingQueryBuilderInitializer : IPagingQueryBuilderInitializer<T>
        {
            public Func<IOrderedContextQueryable<T>, bool, IOrderedContextQueryable<T>> tailSort;
            public PagingQueryBuilder<T> builder;
			public IPagingQueryBuilderInitializer<T> Add(string name, Func<IContextQueryable<T>, bool, IContextQueryable<T>> query, bool defaultOrderDesc = false)
			{

				builder.Fields.Add(
                    name, 
                    new SortField {
                        query = tailSort==null? query:(ctx,o)=>
                        {
                            var r = query(ctx, o);
                            var oa = r as IOrderedContextQueryable<T>;
                            if (oa == null) return r;
                            return tailSort(oa, false);
                        },
                        defaultOrderDesc = defaultOrderDesc
                    });
				return this;
			}
		}
		public PagingQueryBuilder(
			string defaultSortMethod,
			Func<IPagingQueryBuilderInitializer<T>, IPagingQueryBuilderInitializer<T>> initializer
			)
		{
			this.DefaultSortMethod = defaultSortMethod;
			var i = new PagingQueryBuilderInitializer { builder = this };
			initializer(i);
		}
        public PagingQueryBuilder(
            string defaultSortMethod,
            Func<IPagingQueryBuilderInitializer<T>, IPagingQueryBuilderInitializer<T>> initializer,
             Func<IOrderedContextQueryable<T>, bool, IOrderedContextQueryable<T>> tailSort
            )
        {
            this.DefaultSortMethod = defaultSortMethod;
            var i = new PagingQueryBuilderInitializer { builder = this , tailSort = tailSort };
            initializer(i);

        }
        public IContextQueryable<T> Build(IContextQueryable<T> query, Paging paging)
		{
			if (paging == null)
				return query;

			var order = paging.SortOrder;
			if (order == SortOrder.Random)
				throw new NotSupportedException();

			var method = string.IsNullOrWhiteSpace(paging.SortMethod) ? DefaultSortMethod : paging.SortMethod.Trim();
			SortField field;
			if (!Fields.TryGetValue(method, out field))
				throw new NotSupportedException("不支持排序类型：" + method);

			if (order == SortOrder.Default)
				order = field.defaultOrderDesc ? SortOrder.Desc : SortOrder.Asc;

			query = field.query(query, order == SortOrder.Desc);

			query = query.Skip(paging.Offset).Take(paging.Count);
			return query;
		}
		public static PagingQueryBuilder<T> Simple<K>(string Name, Expression<Func<T, K>> Prop, bool defaultOrderDesc = false) =>
			new PagingQueryBuilder<T>(Name, b => b.Add(Name, Prop, defaultOrderDesc));
	}
}
