﻿using SF.Core.ServiceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data
{
	public interface IEntityQueryableProvider
	{
		IContextQueryable<T> Include<T>(IContextQueryable<T> source, string path) where T:class;
		IContextQueryable<T> Include<T, TProperty>(IContextQueryable<T> source, Expression<Func<T, TProperty>> path) where T:class;
	}

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