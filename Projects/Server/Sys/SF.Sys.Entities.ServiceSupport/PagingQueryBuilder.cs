﻿#region Apache License Version 2.0
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace SF.Sys.Entities
{
	public interface IPagingQueryBuilderInitializer<T>
	{
		IPagingQueryBuilderInitializer<T> Add(string name, Func<IQueryable<T>, bool, IQueryable<T>> expr, bool defaultOrderDesc = false);
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
			public Func<IQueryable<T>, bool, IQueryable<T>> query;
        }
		
		string DefaultSortMethod { get; }
		Dictionary<string, SortField> Fields { get; } = new Dictionary<string, SortField>();
		class PagingQueryBuilderInitializer : IPagingQueryBuilderInitializer<T>
        {
            public Func<IOrderedQueryable<T>, bool, IOrderedQueryable<T>> tailSort;
            public PagingQueryBuilder<T> builder;
			public IPagingQueryBuilderInitializer<T> Add(string name, Func<IQueryable<T>, bool, IQueryable<T>> query, bool defaultOrderDesc = false)
			{

				builder.Fields.Add(
                    name, 
                    new SortField {
                        query = tailSort==null? query:(ctx,o)=>
                        {
                            var r = query(ctx, o);
                            var oa = r as IOrderedQueryable<T>;
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
             Func<IOrderedQueryable<T>, bool, IOrderedQueryable<T>> tailSort
            )
        {
            this.DefaultSortMethod = defaultSortMethod;
            var i = new PagingQueryBuilderInitializer { builder = this , tailSort = tailSort };
            initializer(i);

        }
        public IQueryable<T> Build(IQueryable<T> query, Paging paging)
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

			if (order == SortOrder.Default || (int)order==-1)
				order = field.defaultOrderDesc ? SortOrder.Desc : SortOrder.Asc;

			query = field.query(query, order == SortOrder.Desc);

			query = query.Skip(paging.Offset).Take(paging.Count);
			return query;
		}
		public static PagingQueryBuilder<T> Simple<K>(string Name, Expression<Func<T, K>> Prop, bool defaultOrderDesc = false) =>
			new PagingQueryBuilder<T>(Name, b => b.Add(Name, Prop, defaultOrderDesc));
	}
}
