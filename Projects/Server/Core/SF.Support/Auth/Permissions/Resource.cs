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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.Auth.Permissions
{
	public class Resource : IResource
	{
		public Resource(string Id, string Group, string Name, string Description, string[] Operations)
		{
			UIEnsure.HasContent(Id, "ID不能为空");
			UIEnsure.HasContent(Group, "Group不能为空");
			UIEnsure.HasContent(Name, "Name不能为空");
			if (Operations == null || Operations.Length == 0)
				throw new PublicArgumentException("操作不能为空");

			this.Id = Id;
			this.Name = Name;
			this.Group = Group;
			this.Description = Description;
			this.AvailableOperations = Operations;
		}
		public string Description { get; }
		public string Id { get; }
		public string Name { get; }
		public string Group { get; }
		public string[] AvailableOperations { get; }
	}
}
namespace SF.Core.ServiceManagement
{ 
	public static class ResourceExtension
	{
		public static IServiceCollection AddAuthResource(this IServiceCollection sc, Auth.Permissions.IResource resource)
		{
			sc.AddSingleton(resource);
			return sc;
		}
		public static IServiceCollection AddAuthResource(this IServiceCollection sc, string Id, string Group, string Name, string Description, string[] Operations)
		{
			sc.AddAuthResource(new Auth.Permissions.Resource(Id, Group, Name, Description, Operations));
			return sc;
		}
	}
}
