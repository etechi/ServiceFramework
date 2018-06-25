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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Sys.Linq
{
	//public static class ContextQueryableExtension
	//{
	//	class SimpleContextQueryable<T> : IContextQueryable<T>, IOrderedContextQueryable<T>
	//	{
	//		public IQueryableContext Context { get; set; }

	//		public IQueryable<T> Queryable { get; set; }

	//		IOrderedQueryable<T> IOrderedContextQueryable<T>.Queryable => (IOrderedQueryable<T>)Queryable;

	//		IQueryable IContextQueryable.Queryable => Queryable;

	//		public IEnumerator<T> GetEnumerator() => Queryable.GetEnumerator();

	//		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	//	}
	//	class SimpleQueryableContext : IQueryableContext
	//	{
	//		public static SimpleQueryableContext Instance { get; } = new SimpleQueryableContext();
	//		public IOrderedContextQueryable<T> CreateOrderedQueryable<T>(IOrderedQueryable<T> Queryable)
	//		{
	//			return new SimpleContextQueryable<T> { Queryable = Queryable, Context = this };
	//		}

	//		public IContextQueryable<T> CreateQueryable<T>(IQueryable<T> Queryable)
	//		{
	//			return new SimpleContextQueryable<T> { Queryable = Queryable, Context = this };
	//		}
	//	}
	//	public static IContextQueryable<T> AsContextQueryable<T>(this IEnumerable<T> enumerable)
	//	{
	//		return new SimpleContextQueryable<T>
	//		{
	//			Queryable = enumerable.AsQueryable(),
	//			Context = SimpleQueryableContext.Instance
	//		};
	//	}
	
	//}
}
