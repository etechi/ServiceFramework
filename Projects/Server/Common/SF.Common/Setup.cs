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

using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;
using SF.Sys.Services;
using SF.Sys.Hosting;

namespace SF.Sys
{
	public class CommonServicesSetting
	{
		public string BackendConsoleName { get; set; } = "管理控制台";
		public string StaticResIdent { get; set; } = "s0";
		public string StaticResPath { get; set; } = "root://StaticResources";
		public EnvironmentType EnvType { get; set; } = EnvironmentType.Development;

	}
	public static class SetupExtension
	{
		public static IServiceCollection AddCommonServices(
			this IServiceCollection Services,
			CommonServicesSetting Setting
			)
		{
			Services.AddMediaService(
				   Setting.EnvType,
				   true,
				   new Dictionary<string, string>
				   {
						{Setting.StaticResIdent,Setting.StaticResPath},
				   });

			Services.AddIdentityServices(
				null,
				"acc",
				DefaultUserIcon: Setting.StaticResIdent + "-ef-identity-user-png"
				);

			Services.AddNotificationServices();
			Services.AddShortLinkService();

			Services.AddAdminServices();
			Services.AddFrontEndServices(Setting.EnvType);
			Services.AddFriendlyFrontEndServices();

			Services.AddDefaultPhoneNumberValidator();
			Services.AddDocumentServices();
			Services.AddTicketServices();
			Services.AddConversationServices();

			Services.AddBackEndConsoleServices(Setting.BackendConsoleName);
			return Services;

		}
	}
}
