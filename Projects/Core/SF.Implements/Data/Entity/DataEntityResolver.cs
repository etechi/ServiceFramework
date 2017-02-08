using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Data.Entity
{
	class DefaultDataEntityResolver : IDataEntityResolver
	{
		IServiceProvider ServiceProvider { get; }
		DataEntityConfigCache ConfigCache { get; }
		public DefaultDataEntityResolver(IServiceProvider ServiceProvider,DataEntityConfigCache ConfigCache)
		{
			this.ServiceProvider = ServiceProvider;
			this.ConfigCache = ConfigCache;
		}
		public async Task<IDataEntity[]> Resolve(string Type, string[] Keys)
		{
			var ci=ConfigCache.FindConfigItem(Type);
			if (ci == null)
				return Array.Empty<IDataEntity>();
			return await ci.Load(ServiceProvider, Keys);
		}

	}
}
