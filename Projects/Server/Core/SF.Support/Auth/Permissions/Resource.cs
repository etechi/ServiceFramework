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
