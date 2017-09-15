using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Entities
{
	class DataEntity : IDataEntity
	{
		public object Instance { get; }
		public DataEntityConfigItem Config { get; }
		public DataEntity(object Instance, DataEntityConfigItem Config)
		{
			this.Instance = Instance;
			this.Config = Config;
		}
		public string Ident => Config.GetIdent(Instance);
		public string Name => Config.GetName(Instance) ?? Ident;
	}
}
