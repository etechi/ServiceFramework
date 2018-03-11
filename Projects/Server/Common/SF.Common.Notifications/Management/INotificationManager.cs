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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SF.Common.Notifications.Management
{
	
	/// <summary>
	/// 通知管理
	/// </summary>
	[EntityManager]
	[DefaultAuthorize(PredefinedRoles.客服专员, true)]
	[DefaultAuthorize(PredefinedRoles.运营专员)]

	[NetworkService]
	public interface INotificationManager : 
		IEntitySource<ObjectKey<long>,Notification,NotificationQueryArgument>,
		IEntityManager<ObjectKey<long>,NotificationEditable>
	{
		Task<long> FindPolicy(string Ident);
	}

	public static class NotificationManagerExtension
	{
		public static Task<long> CreateBroadcastNotification(
			   this INotificationManager Manager,
			   string Policy,
			   Dictionary<string, object> Args,
			   DateTime Time,
			   DateTime Expires = default,
			   string BizIdent = null,
			   string Image = null,
			   string Link = null,
			   string Name = null,
			   string Content = null
			   )
		{
			return Manager.CreateNotification(
				NotificationMode.Boardcast,
				null,
				Policy,
				Args,
				Time,
				Expires == default ? Time.AddYears(1) : Expires,
				BizIdent,
				Image,
				Link,
				Name,
				Content
				);
		}
		public static Task<long> CreateNormalNotification(
			this INotificationManager Manager,
			long? Target,
			string Policy,
			Dictionary<string, object> Args,
			DateTime Time=default,
			DateTime Expires = default,
			string BizIdent = null,
			string Image = null,
			string Link = null,
			string Name = null,
			string Content = null
			)
		{
			return Manager.CreateNotification(
				NotificationMode.Normal,
				Target.HasValue?new[] { Target.Value }:null,
				Policy,
				Args,
				Time,
				Expires,
				BizIdent,
				Image,
				Link,
				Name,
				Content
				);

		}
		public static  Task<long> CreateNormalNotification(
			this INotificationManager Manager,
			IEnumerable<long> Targets,
			string Policy,
			Dictionary<string, object> Args,
			DateTime Time=default,
			DateTime Expires=default,
			string BizIdent = null,
			string Image=null,
			string Link=null,
			string Name=null,
			string Content=null
			)
		{
			return Manager.CreateNotification(
				NotificationMode.Normal,
				Targets,
				Policy,
				Args,
				Time,
				Expires,
				BizIdent,
				Image,
				Link,
				Name,
				Content
				);

		}
		public static async Task<long> CreateNotification(
			this INotificationManager Manager,
			NotificationMode Mode,
			IEnumerable<long> Targets,
			string Policy,
			Dictionary<string, object> Args,
			DateTime Time=default,
			DateTime Expires=default,
			string BizIdent = null,
			string Image=null,
			string Link=null,
			string Name=null,
			string Content=null
			)
		{
			var policyId = Policy == null ? (long?)null : await Manager.FindPolicy(Policy);
			var re = await Manager.CreateAsync(new NotificationEditable
			{
				Args = Args,
				BizIdent = BizIdent,
				Time = Time,
				Expires = Expires,
				Image = Image,
				Link = Link,
				Mode = Mode,
				PolicyId = policyId,
				Targets = Targets,
				TargetId = Targets == null ? (long?)null : Targets.FirstOrDefault(),
				Content=Content,
				Name=Name
			});
			return re.Id;
		}
	}
}
