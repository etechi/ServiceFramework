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


using SF.Common.TextMessages;
using SF.Common.TextMessages.EMailProviders;
using SF.Common.TextMessages.Management;

namespace SF.Sys.Services
{
	public static class TextMessageServicesDIExtension
	{
		
		public static IServiceCollection AddTextMessageServices(
			this IServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.AddDataModules<
				SF.Common.TextMessages.Management.DataModels.MsgActionRecord,
				SF.Common.TextMessages.Management.DataModels.MsgPolicy,
				SF.Common.TextMessages.Management.DataModels.MsgRecord
				>(
				TablePrefix??"Common"
				);
			sc.EntityServices(
				"TextMessage",
				"文本消息",
				d => 
				d.Add<IMsgRecordManager, EntityMsgRecordManager>("TextMessageRecord","文本消息记录")
				.Add<IMsgActionRecordManager, EntityMsgActionRecordManager>("TextMessageActionRecord", "文本消息发送录")
				.Add<IMsgPolicyManager, EntityMsgPolicyManager>("TextMessagePolicy", "文本消息发生策略")
				);

			sc.AddSingleton<IMsgArgumentFactory, MsgArgumentFactory>();
			sc.AddSingleton<IMsgLogger,EntityMsgLogger>();

			sc.AddManagedTransient<IMsgProvider, SystemEMailProvider>();
			sc.AddManagedScoped<ITextMessageService, MsgService>();
			sc.AddManagedScoped<IMsgProvider, DebugMsgProvider>();
			sc.AddTransient(sp => (IDebugMsgProvider)sp.Resolve<IMsgProvider>("debug"));

			sc.InitServices("文本消息服务",async (sp, sim, scope) =>
			 {
				 await sim.DefaultService<ITextMessageService, MsgService>(new MsgServiceSetting { }).Ensure(sp, scope);
				 await sim.DefaultService<IMsgPolicyManager, EntityMsgPolicyManager>(null).Ensure(sp, scope);
				 await sim.DefaultService<IMsgRecordManager, EntityMsgRecordManager>(null).Ensure(sp, scope);
				 await sim.DefaultService<IMsgActionRecordManager, EntityMsgActionRecordManager>(null).Ensure(sp, scope);

				 await sim.Service<IMsgProvider, DebugMsgProvider>(null).WithIdent("debug").Ensure(sp, scope);
				 await sim.Service<IMsgProvider, SystemEMailProvider>(null).WithIdent("email").Ensure(sp, scope);
			 });
			
			return sc;
		}
		//public static IServiceCollection AddSimPhoneTextMessageService(this IServiceCollection sc)
		//{
		//	sc.AddManagedScoped<IPhoneMessageService, SimPhoneTextMessageService>();
		//	return sc;
		//}
		
	}
}