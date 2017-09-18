using SF.Auth.Permissions;
using SF.Entities;
using System;
using System.Linq;

namespace SF.Auth.Permissions
{
	public class Operation : IOperation
	{
		public Operation(string Id, string Name, string Description)
		{
			this.Id = Id;
			this.Name = Name;
			this.Description = Description;
		}
		public string Description { get; }
		public string Id { get; }
		public string Name { get; }
	}
}
namespace SF.Core.ServiceManagement
{ 
	public static class OperationCollectionExtension
	{
		public static IServiceCollection AddAuthOperation(this IServiceCollection sc, IOperation operation)
		{
			sc.AddSingleton(operation);
			return sc;
		}
		public static IServiceCollection AddAuthOperation(this IServiceCollection sc, string Id, string Name, string Description)
		{
			return sc.AddAuthOperation(new Operation(Id, Name, Description));
		}
		public static IServiceCollection EnsureAuthOperation(this IServiceCollection sc, string Id, string Name, string Description)
		{
			if (sc.Any(s => 
				s.InterfaceType == typeof(IOperation) && 
				s.ServiceImplementType == ServiceImplementType.Instance && 
				((IOperation)s.ImplementInstance).Id == Id)
				)
				return sc;
			return sc.AddAuthOperation(Id, Name, Description);
		}
	}
}
