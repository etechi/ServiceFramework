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

using SF.Common.Tickets.Management;
using SF.Common.Tickets;
using SF.Sys.Services.Management;
using SF.Sys.Entities.AutoTest;
using SF.Sys.Entities.AutoEntityProvider;
using SF.Sys.Entities;
using SF.Sys.BackEndConsole;

namespace SF.Sys.Services
{
	public static class TicketDIExtension
		
	{
		public static IServiceCollection AddTicketServices(this IServiceCollection sc,string TablePrefix=null)
		{
			//文章
			sc.EntityServices(
				"Ticket",
				"工单管理",
				d => d.Add<ITicketCategoryManager, TicketCategoryManager>("TicketCategory","工单分类",typeof(TicketCategory))
					.Add<ITicketManager, TicketManager>("Ticket", "工单", typeof(Common.Tickets.Management.TicketEditable))
					.Add<ITicketReplyManager, TicketReplyManager>("TicketReply", "工单回复", typeof(Common.Tickets.Management.TicketReply))
				//.Add<ITicketService, TicketService>()
				);

			sc.AddManagedScoped<Common.Tickets.Front.ITicketService, Common.Tickets.Front.TicketService>(IsDataScope: true);

			//sc.GenerateEntityManager("TicketCategory");
			//sc.GenerateEntityManager("Ticket");

			//sc.AddAutoEntityType(
			//	(TablePrefix ?? "") + "Doc",
			//	false,
			//	typeof(Ticket),
			//	typeof(TicketInternal),
			//	typeof(TicketEditable),
			//	typeof(Category),
			//	typeof(CategoryInternal)
			//	);


			sc.AddDataModules<
				SF.Common.Tickets.DataModels.DataTicket,
				SF.Common.Tickets.DataModels.DataTicketCategory,
				SF.Common.Tickets.DataModels.DataTicketReply
				>(TablePrefix ?? "Common");

			//sc.AddAutoEntityTest(NewTicketManager);
			//sc.AddAutoEntityTest(NewTicketCategoryManager);
			sc.InitServices("Tickets", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<ITicketManager, TicketManager>(null)
					.WithConsolePages("前端内容/工单管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<ITicketCategoryManager, TicketCategoryManager>(null)
					.WithConsolePages("前端内容/工单管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<ITicketReplyManager, TicketReplyManager>(null)
					.WithConsolePages("前端内容/工单管理")
					.Ensure(sp, parent);

				 await sim.DefaultService<Common.Tickets.Front.ITicketService, Common.Tickets.Front.TicketService>(null)
					 .Ensure(sp, parent);
			 });
			sc.AddInitializer("data","工单分类数据",async sp=>{
				 var tcm = sp.Resolve<ITicketCategoryManager>();
				 await tcm.EnsureEntity(
					 await tcm.QuerySingleEntityIdent(new TicketCategoryQueryArgument { Name="默认分类"}),
					 s =>
					 {
						 s.Name = "默认分类";
					 });

			 });
			
			return sc;
		}
	}
}
