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

using SF.Sys.Entities;
using SF.Sys.BackEndConsole;
using SF.Biz.Accounting;

namespace SF.Sys.Services
{
    public static class AccountingDIExtension
		
	{
		public static IServiceCollection AddAccountingServices(this IServiceCollection sc, string TablePrefix = null)
		{
			//财务
			sc.EntityServices(
				"Accounting",
				"财务管理",
				d => d.Add<IAccountManager, AccountManager>("Account", "账户", typeof(Account))
                    .Add<IAccountTitleManager, AccountTitleManager>("AccountTitle", "账户科目", typeof(AccountTitle))
                    .Add<ITransferRecordManager, TransferRecordManager>("TransferRecord", "转账记录", typeof(TransferRecord))
                    .Add<IDrawbackRecordManager, DrawbackRecordManager>("DrawbackRecord", "退款记录", typeof(DrawbackRecord))
                    .Add<ITransferRecordManager, TransferRecordManager>("TransferRecord", "转账记录", typeof(TransferRecord))
                );

			sc.AddManagedScoped<IDepositService, DepositService>();
            sc.AddManagedScoped<IDrawbackService, DrawbackService>();
            sc.AddManagedScoped<ITransferService, TransferService>();
            sc.AddManagedScoped<IAccountService, AccountService>();
            sc.AddManagedScoped<IAccountingService, AccountingService>();


            sc.AddDataModules<
				SF.Biz.Accounting.DataModels.DataAccount,
                SF.Biz.Accounting.DataModels.DataAccountTitle,
                SF.Biz.Accounting.DataModels.DataDepositRecord,
                SF.Biz.Accounting.DataModels.DataDrawbackRecord,
                SF.Biz.Accounting.DataModels.DataTransferRecord,
                SF.Biz.Accounting.DataModels.DataTransferRecordItem
                >(TablePrefix ?? "Biz");

			sc.InitServices("Accounting", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<IAccountManager, AccountManager>(null)
					.WithConsolePages("财务管理/账户管理")
					.Ensure(sp, parent);

                 await sim.DefaultService<IAccountTitleManager, AccountTitleManager>(null)
                    .WithConsolePages("财务管理/账户科目管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<IDepositRecordManager, DepositRecordManager>(null)
                    .WithConsolePages("财务管理/充值管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<IDrawbackRecordManager, DrawbackRecordManager>(null)
                    .WithConsolePages("财务管理/退款管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<ITransferRecordManager, TransferRecordManager>(null)
                    .WithConsolePages("财务管理/转账管理")
                    .Ensure(sp, parent);

             });


			return sc;
		}
	}
}
