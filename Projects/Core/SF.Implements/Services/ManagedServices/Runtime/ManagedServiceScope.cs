using System;
using System.Collections.Generic;
namespace SF.Services.ManagedServices.Runtime
{
	class ManagedServiceScope : IManagedServiceScope, IDisposable
	{
		IManagedServiceFactoryManager FactoryManager { get; }
		Dictionary<Tuple<Type, string>, object> _Services;
		public ManagedServiceScope(IManagedServiceFactoryManager FactoryManager)
		{
			this.FactoryManager = FactoryManager;
		}
		
		public object Resolve(IServiceProvider sp, Type Type, string Id)
		{
			var factory = FactoryManager.GetServiceFactory(sp, Type, Id);
			if (!factory.IsScopedLifeTime)
				return factory.Create(sp, this);

			if (_Services == null)
				_Services = new Dictionary<Tuple<Type, string>, object>();
			var key = Tuple.Create(Type, Id ?? string.Empty);
			object s;
			if (_Services.TryGetValue(key, out s))
				return s;
			return _Services[key]= factory.Create(sp,this);
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
