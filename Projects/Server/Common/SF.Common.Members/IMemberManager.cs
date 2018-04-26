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



using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;

namespace SF.Common.Members
{
	public class MemberQueryArgument : QueryArgument<ObjectKey<long>>
	{
		[StringContains]
		public string Ident { get; set; }
		[StringContains]
		public string Name { get; set; }
	}

	/// <summary>
	/// 会员
	/// </summary>
	/// <typeparam name="TInternal"></typeparam>
	/// <typeparam name="TEditable"></typeparam>
	/// <typeparam name="TQueryArgument"></typeparam>
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员)]
	[DefaultAuthorize(PredefinedRoles.媒介专员, true)]
	[DefaultAuthorize(PredefinedRoles.运营专员,true)]
	[DefaultAuthorize(PredefinedRoles.系统管理员)]
	[NetworkService]
	[Category("用户管理", "会员管理")]
	public interface IMemberManager<TInternal,TEditable,TQueryArgument>: 
		IEntitySource<long,TInternal,TQueryArgument>,
		IEntityManager<long, TEditable>
		where TInternal:Models.MemberInternal
		where TEditable: Models.MemberEditable
		where TQueryArgument:MemberQueryArgument
    {
	}

	public interface IMemberManager: IMemberManager<Models.MemberInternal, Models.MemberEditable, MemberQueryArgument>
	{

	}

}

