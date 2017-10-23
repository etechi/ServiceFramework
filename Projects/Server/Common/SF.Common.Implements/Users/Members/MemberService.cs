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

using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;
using SF.Core.ServiceManagement;

namespace SF.Users.Members
{
	public class MemberService :
		MemberService<DataModels.Member>
	{
		public MemberService(IMemberManagementService UserManagerService, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(UserManagerService, ServiceInstanceDescriptor)
		{
		}
	}
	public class MemberService<TMember> :
		Auth.Users.BaseUserService<MemberDesc, MemberInternal, MemberEditable, MemberQueryArgument, TMember, IMemberManagementService>,
		IMemberService
		where TMember:DataModels.Member<TMember>,new()
	{
		public MemberService(IMemberManagementService UserManagerService, IServiceInstanceDescriptor ServiceInstanceDescriptor) : base(UserManagerService, ServiceInstanceDescriptor)
		{
		}
	}

}

