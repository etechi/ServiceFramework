using System;
using System.Collections.Generic;
using System.Text;

namespace SF.DI
{
	public interface IServiceResolver
	{	
		object Resolve(Type Type);
	}
	
	public interface IDIScope : 
		IDisposable
	{
		IServiceResolver Resolver { get; }
	}
	public interface IDIScopeFactory
	{
		IDIScope BeginScope();
	}
	public static class ServiceResolver
	{
		public static T Resolve<T>(this IServiceResolver Resolver)
		{
			return (T)Resolver.Resolve(typeof(T));
		}
	}
}
