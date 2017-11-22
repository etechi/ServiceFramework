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

using SF.Sys.Collections.Generic;
using SF.Sys.Services.Internals;
using System;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SF.Sys.Services
{
	public static class PublicServiceFactory
	{

		public static (IServiceDeclaration, IServiceImplement) ResolveMetadata(
			IServiceResolver ServiceResolver,
			long Id,
			string svcTypeName,
			string implTypeName,
			Type ServiceType
			) => Internals.ServiceFactory.ResolveMetadata(
				ServiceResolver,
				Id,
				svcTypeName,
				implTypeName,
				ServiceType
				);

		public static IServiceFactory Create(
			long Id,
			long? ParentId,
			Func<long?> LazyDataScopeId,
			IServiceDeclaration decl,
			IServiceImplement impl,
			Type ServiceType,
			IServiceMetadata ServiceMetadata,
			string Setting
			) => Internals.ServiceFactory.Create(
				Id,
				ParentId,
				LazyDataScopeId,
				decl,
				impl,
				ServiceType,
				null,
				ServiceMetadata,
				Setting
				);
		}

}
