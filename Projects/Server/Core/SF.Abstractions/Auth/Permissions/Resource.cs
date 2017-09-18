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
        IReadOnlyCollection<IOperation> AvailableOperations { get; }
    }
    public interface IResourceCollection
    {
        void Add(IResource resource);
        IResource Get(string Id);
    }
	public class Resource : IResource
	{
		public Resource(string Id, string Group, string Name, string Description, IReadOnlyCollection<IOperation> Operations)
		{
			UIEnsure.HasContent(Id, "ID不能为空");
			UIEnsure.HasContent(Group, "Group不能为空");
			UIEnsure.HasContent(Name, "Name不能为空");
			if (Operations == null || Operations.Count == 0)
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
		public IReadOnlyCollection<IOperation> AvailableOperations { get; }
	}
	public static class ResourceCollectionExtension
    {
        public static IServiceProvider AddAuthResource(this IServiceProvider Provider, IResource resource)
        {
            Provider.Resolve<IResourceCollection>().Add(resource);
            return Provider;
        }
        public static IServiceProvider AddAuthResource(this IServiceProvider Provider, string Id,string Group,string Name,string Description, IReadOnlyCollection<IOperation> Operations )
        {
            Provider.AddAuthResource(new Resource(Id,Group, Name, Description, Operations));
            return Provider;
        }
        public static IServiceProvider AddAuthResource(this IServiceProvider Provider, string Id, string Group,string Name, string Description, params string[] Operations)
        {
            var oc = Provider.Resolve<IOperationCollection>();
            var os = Operations.Select(o =>
            {
                var r = oc.Get(o);
                if (r == null)
                    throw new ArgumentException("找不到操作：" + o);
                return r;
            }).ToArray();
            return Provider.AddAuthResource(Id, Group, Name, Description, os);
        }
    }
}
