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


using SF.Core.ServiceManagement;
using SF.Core.ServiceManagement.Management;
using SF.Entities;
using SF.Common.Members;
using SF.Common.Members.Models;
using System;
using System.Collections.Generic;

namespace SF.Core.ServiceManagement
{
	public static class MemberServiceDIExtension
	{
		public static IServiceCollection AddMemberManager<TService, TImplement, TInternal, TEditable, TQueryArgument, TMember>(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			where TInternal : SF.Common.Members.Models.MemberInternal, new()
			where TEditable : SF.Common.Members.Models.MemberEditable, new()
			where TQueryArgument : SF.Common.Members.MemberQueryArgument, new()
			where TMember : SF.Common.Members.DataModels.Member<TMember>, new()
			where TService : class, IMemberManager<TInternal, TEditable, TQueryArgument>
			where TImplement : MemberManager<TInternal, TEditable, TQueryArgument, TMember>, TService
		{
			sc.AddDataModules<TMember>(TablePrefix);
			sc.EntityServices(
				"Member",
				"会员",
				d => d.Add<TService, TImplement>("Member","会员")
				);
			return sc;
		}

		public static IServiceCollection AddMemberServices(
			this IServiceCollection sc,
			//Func<MenuItem[]> DefaultMenu=null,
			string TablePrefix = null
			)
			=> sc.AddMemberManager<
				IMemberManager,
				MemberManager,
				MemberInternal,
				MemberEditable,
				MemberQueryArgument,
				SF.Common.Members.DataModels.Member
				>(TablePrefix);

		public static IServiceInstanceInitializer<TService> NewMemberService<TService, TImplement, TInternal, TEditable, TQueryArgument, TMember>(
			this IServiceInstanceManager sim
			)
			where TInternal : SF.Common.Members.Models.MemberInternal, new()
			where TEditable : SF.Common.Members.Models.MemberEditable, new()
			where TQueryArgument : SF.Common.Members.MemberQueryArgument, new()
			where TMember : SF.Common.Members.DataModels.Member<TMember>, new()
			where TService : class, IMemberManager<TInternal, TEditable, TQueryArgument>
			where TImplement : MemberManager<TInternal, TEditable, TQueryArgument, TMember>, TService
			=> sim.DefaultService<TService, TImplement>(
				new { }
				).WithMenuItems("会员");

		public static IServiceInstanceInitializer<IMemberManager> NewMemberService(
			this IServiceInstanceManager sim
			)
			=> sim.DefaultService<IMemberManager, MemberManager>(
				new { }
				).WithMenuItems("会员");
	}
}