using System;
using System.Collections.Generic;
using System.Text;

namespace SF.DI
{

	public enum ServiceLifetime
	{
		Singleton = 0,
		Scoped = 1,
		Transient = 2
	}
	public interface IServiceCollection
    {
		void AddSingleton(Type Interface,object Implement);
		void Add(Type Interface, Type Implement, ServiceLifetime LiftTime);
		void Add(Type Interface, Func<IServiceResolver, object> ImplementCreator, ServiceLifetime LiftTime);
	}
}
