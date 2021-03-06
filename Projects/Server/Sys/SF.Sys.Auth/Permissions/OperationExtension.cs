﻿#region Apache License Version 2.0
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

using SF.Sys.Auth.Permissions;
using SF.Sys.Services;
using System;
using System.Linq;

namespace SF.Sys.Auth.Permissions
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
namespace SF.Sys.Services
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
