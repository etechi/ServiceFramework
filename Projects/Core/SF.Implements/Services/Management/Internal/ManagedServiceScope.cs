using System;
using System.Collections.Generic;
namespace SF.Services.Management.Internal
{
	class ManagedServiceScope : IManagedServiceScope, IDisposable
	{
		IManagedServiceFactory Factory { get; }
		Dictionary<Tuple<Type, string>, object> _Services;
		public ManagedServiceScope(IManagedServiceFactory Factory)
		{
			this.Factory = Factory;
		}
		
		public object Resolve(IServiceProvider sp, Type Type, string Id)
		{
			if (_Services == null)
				_Services = new Dictionary<Tuple<Type, string>, object>();
			var key = Tuple.Create(Type, Id ?? string.Empty);
			object s;
			if (_Services.TryGetValue(key, out s))
				return s;
			return _Services[key]=Factory.Create(sp,this, Type, Id);
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
