using System;
using System.Collections.Generic;
namespace SF.Core.ManagedServices.Runtime
{
	class ManagedServiceScope : IManagedServiceScope, IDisposable
	{
		IManagedServiceFactoryManager FactoryManager { get; }
		Dictionary<Tuple<Type, string>, object> _Services;
		IServiceProvider ServiceProvider { get; }
		public ManagedServiceScope(IManagedServiceFactoryManager FactoryManager, IServiceProvider ServiceProvider)
		{
			this.FactoryManager = FactoryManager;
			this.ServiceProvider = ServiceProvider;
		}
		
		public object Resolve(Type Type, string Id)
		{
			var factory = FactoryManager.GetServiceFactory(ServiceProvider, Type, Id);
			if (!factory.IsScopedLifeTime)
				return factory.Create(ServiceProvider, this);

			if (_Services == null)
				_Services = new Dictionary<Tuple<Type, string>, object>();
			var key = Tuple.Create(Type, Id ?? string.Empty);
			object s;
			if (_Services.TryGetValue(key, out s))
				return s;
			return _Services[key]= factory.Create(ServiceProvider, this);
		}

		public void Dispose()
		{
			if(_Services != null)
				foreach (var v in _Services.Values)
				{
					var p = v as IDisposable;
					if (p != null)
						p.Dispose();
				}
		}
	}

}
