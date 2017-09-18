using SF.Entities;
using System;

namespace SF.Auth.Permissions
{
	public interface IOperation : IEntityWithId<string>
	{
        string Name { get; }
        string Description { get; }
	}
  
    //public interface IOperationCollection
    //{
    //    void Add(IOperation operation);
    //    IOperation Get(string Id);
    //}
	//public class Operation : IOperation
	//{
	//	public Operation(string Id, string Name, string Description)
	//	{
	//		this.Id = Id;
	//		this.Name = Name;
	//		this.Description = Description;
	//	}
	//	public string Description { get; }
	//	public string Id { get; }
	//	public string Name { get; }
	//}
	//public static class OperationCollectionExtension
 //   {
 //       public static void AddAuthOperation(this IServiceProvider Provider, IOperation operation)
 //       {
 //           Provider.Resolve<IOperationCollection>().Add(operation);
 //       }
 //       public static void AddAuthOperation(this IServiceProvider Provider, string Id,string Name,string Description)
 //       {
 //           Provider.AddAuthOperation(new Operation(Id, Name, Description));
 //       }
 //       public static void EnsureAuthOperation(this IServiceProvider Provider, string Id, string Name, string Description)
 //       {
 //           var oc = Provider.Resolve<IOperationCollection>();
 //           if (oc.Get(Id) != null)
 //               return;
 //           AddAuthOperation(Provider, Id, Name, Description);
 //       }
 //   }
}
