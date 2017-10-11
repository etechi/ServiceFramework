using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using SF.Core.Logging;
using SF.Core.ServiceManagement;
using SF.Core.Events;
using SF.Entities;
using SF.Core;

namespace SF.Management.FrontEndContents.Runtime
{
	public class EntityDataProvider : IDataProvider
	{
		public ILogger<EntityDataProvider> Logger { get; }
		public EntityDataProvider(ILogger<EntityDataProvider> Logger)
		{
			this.Logger = Logger;
		}

		public Task<object> Load(IContent content, string contentConfig)
		{
			return Load(content.ProviderConfig, contentConfig);
		}
		public async Task<object> Load(string contentConfig, string blockConfig)
		{
			if (string.IsNullOrWhiteSpace(contentConfig))
				return Task.FromResult<object>(null);

			var cfgs = Json.Parse<Dictionary<string, string>>(contentConfig);
			if (!string.IsNullOrWhiteSpace(blockConfig))
			{
				var block_cfgs = Json.Parse<Dictionary<string, string>>(blockConfig);
				foreach (var p in block_cfgs)
					cfgs[p.Key] = p.Value;
			}
		
		}
	}

}
}
