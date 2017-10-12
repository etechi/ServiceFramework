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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SF.Auth.Permissions.Models;
using SF.Entities;
namespace SF.Auth.Permissions
{
	public class ResourceDictionary:
		Dictionary<string, ResourceInternal>,
		IReadOnlyDictionary<string, ResourceInternal>
	{
		public ResourceDictionary(IEnumerable<IResource> items, IReadOnlyDictionary<string, OperationInternal> Operations)
		{
			foreach (var it in items)
				Add(it.Id, new ResourceInternal
				{
					Id = it.Id,
					CreatedTime = DateTime.Now,
					LogicState = EntityLogicState.Enabled,
					Name = it.Name,
					UpdatedTime = DateTime.Now,
					Group = it.Group,
					AvailableOperations = it.AvailableOperations.Select(o =>
						Operations.Get(o)??throw new ArgumentException($"构造资源时，发现未定义的操作:{o}")
					).ToArray()
				});
		}
	}
}
