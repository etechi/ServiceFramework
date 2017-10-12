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


using SF.Core.ServiceManagement.Management;
using SF.Common.TextMessages;
using System.Linq;
using SF.Metadata;
using SF.Core.NetworkService.Metadata;
using SF.Entities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using SF.Common.TextMessages.Management;

namespace SF.Core.ServiceManagement
{
	public static class TextMessageServicesDIExtension
	{
		
		public static IServiceCollection AddMsgRecordManager(
			this IServiceCollection sc
			)
		{
			sc.AddDataModules<SF.Common.TextMessages.Management.DataModels.TextMessageRecord>();
			sc.EntityServices(
				"TextMessageRecord",
				"文本消息记录",
				d => d.Add<IMsgRecordManager, EntityMsgRecordManager>()
				);

			sc.AddTransient(sp => (ITextMessageLogger)sp.Resolve<IMsgRecordManager>());

			//sc.AddInitializer(
			//	"初始化菜单",
			//	sp=>Init(sp,DefaultMenu),
			//	int.MaxValue
			//	);
			return sc;
		}
		public static IServiceCollection AddSimPhoneTextMessageService(this IServiceCollection sc)
		{
			sc.AddManagedScoped<IPhoneMessageService, SimPhoneTextMessageService>();
			return sc;
		}
		public static IServiceCollection AddTextMessageServices(this IServiceCollection sc)
		{
			sc.AddMsgRecordManager();
			sc.AddSimPhoneTextMessageService();

			return sc;
		}

	}
}