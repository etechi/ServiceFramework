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
	
    public interface IResource : IEntityWithId<string>
    {
        string Group { get; }
        string Name { get; }
        string Description { get; }
        string[] AvailableOperationScopes { get; }
    }
 //   public interface IResourceCollection
 //   {
 //       void Add(IResource resource);
 //       IResource Get(string Id);
 //   }
	//public class Resource : IResource
	//{
	//	public Resource(string Id, string Group, string Name, string Description, IReadOnlyCollection<IOperation> Operations)
	//	{
	//		UIEnsure.HasContent(Id, "ID不能为空");
	//		UIEnsure.HasContent(Group, "Group不能为空");
	//		UIEnsure.HasContent(Name, "Name不能为空");
	//		if (Operations == null || Operations.Count == 0)
	//			throw new PublicArgumentException("操作不能为空");

	//		this.Id = Id;
	//		this.Name = Name;
	//		this.Group = Group;
	//		this.Description = Description;
	//		this.AvailableOperations = Operations;
	//	}
	//	public string Description { get; }
	//	public string Id { get; }
	//	public string Name { get; }
	//	public string Group { get; }
	//	public IReadOnlyCollection<IOperation> AvailableOperations { get; }
	//}
	//public static class ResourceCollectionExtension
 //   {
 //       public static IServiceProvider AddAuthResource(this IServiceProvider Provider, IResource resource)
 //       {
 //           Provider.Resolve<IResourceCollection>().Add(resource);
 //           return Provider;
 //       }
 //       public static IServiceProvider AddAuthResource(this IServiceProvider Provider, string Id,string Group,string Name,string Description, IReadOnlyCollection<IOperation> Operations )
 //       {
 //           Provider.AddAuthResource(new Resource(Id,Group, Name, Description, Operations));
 //           return Provider;
 //       }
 //       public static IServiceProvider AddAuthResource(this IServiceProvider Provider, string Id, string Group,string Name, string Description, params string[] Operations)
 //       {
 //           var oc = Provider.Resolve<IOperationCollection>();
 //           var os = Operations.Select(o =>
 //           {
 //               var r = oc.Get(o);
 //               if (r == null)
 //                   throw new ArgumentException("找不到操作：" + o);
 //               return r;
 //           }).ToArray();
 //           return Provider.AddAuthResource(Id, Group, Name, Description, os);
 //       }
 //   }
}
