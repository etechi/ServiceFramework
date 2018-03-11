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


using SF.Common.Notifications.Models;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Entities;
using SF.Sys.NetworkService;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Management
{
	/// <summary>
	/// 通知策略管理
	/// </summary>
	[EntityManager]
	[DefaultAuthorizeAttribute("admin")]
	[NetworkService]
	public interface INotificationSendPolicyManager :
		IEntitySource<ObjectKey<long>, NotificationSendPolicy, NotificationSendPolicyQueryArgument>,
		IEntityManager<ObjectKey<long>, NotificationSendPolicy>
	{
	}

	public static class NotificationSendPolicyManagerExtension
	{
		public static async Task EnsurePolicy(
			this INotificationSendPolicyManager Manager,
			string Ident,
			string Name,
			string NameTemplate,
			string ContentTemplate,
			params MessageSendAction[] Actions)
		{
			await Manager.EnsureEntity(
				await Manager.QuerySingleEntityIdent(new NotificationSendPolicyQueryArgument { Ident = Ident }),
				p =>
				{
					p.NameTemplate = NameTemplate;
					p.ContentTemplate = ContentTemplate;
					p.Ident = Ident;
					p.Name = Name;
					p.Actions = Actions;
				}
			);
		}
	}
}
