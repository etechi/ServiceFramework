using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using SF.Core.ServiceManagement;

namespace SF.Data
{
	public interface IEntityDataModelSource
	{
		EntityDataModels DataModels { get; }
	}
	public class EntityDataModels
	{
		public string Prefix { get; }
		public Type[] Types { get; }
		public EntityDataModels(Type[] Types,string Prefix)
		{
			this.Types = Types;
			this.Prefix = Prefix; 
		}
	}
}
