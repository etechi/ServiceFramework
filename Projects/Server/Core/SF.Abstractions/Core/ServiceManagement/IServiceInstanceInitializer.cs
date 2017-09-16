using SF.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	public class ServiceInstanceConfig
	{
		public string Name { get; set; }
		public string Title { get; set; }
		public string Description { get; set; }
		public string Ident { get; set; }
		public EntityLogicState LogicState { get; set; } = EntityLogicState.Disabled;
	}
	public interface IServiceInstanceInitializer
	{
		ServiceInstanceConfig Config { get; }
		Task<long> Ensure(IServiceProvider ServiceProvider, long? ParentId);
	}
	public interface IServiceInstanceInitializer<T> : IServiceInstanceInitializer
	{

	}
}
