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

namespace SF.Auth.Permissions
{
	public interface IOperationScope : IEntityWithId<string>
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
