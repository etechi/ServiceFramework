#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

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
