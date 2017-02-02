using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	public static class ContextQueryableExtension
	{
		
		public static IContextQueryable<T> Include<T>(this IContextQueryable<T> source, string path) where T : class
		{
			return ((IDataContextProvider)source.Context).EntityQueryableProvider.Include(source, path);
		}
		public static IContextQueryable<T> Include<T, TProperty>(this IContextQueryable<T> source, Expression<Func<T, TProperty>> path) where T : class
		{
			return ((IDataContextProvider)source.Context).EntityQueryableProvider.Include(source, path);
		}

	}
}
