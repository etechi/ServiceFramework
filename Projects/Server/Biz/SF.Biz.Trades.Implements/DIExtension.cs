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

using SF.Biz.Trades;
using SF.Biz.Trades.Managements;
using SF.Biz.Trades.StateProviders;
using SF.Sys.BackEndConsole;

namespace SF.Sys.Services
{
    public static class TradeCartDIExtension
		
	{
		public static IServiceCollection AddTradeServices(this IServiceCollection sc, string TablePrefix = null)
		{
            //交易
            sc.EntityServices(
				"Trades",
                "交易管理",
				d => d.Add<ITradeManager, TradeManager>("Trade", "交易", typeof(SF.Biz.Trades.Trade))
                    .Add<ITradeItemManager, TradeItemManager>("TradeItem", "交易明细", typeof(SF.Biz.Trades.TradeItem))

                );

			sc.AddManagedScoped<IBuyerTradeService, BuyerTradeService>();
            sc.AddManagedScoped<ISellerTradeService, SellerTradeService>();
            sc.AddSingleton<ITradeSyncQueue, TradeSyncQueue>();

            sc.AddManagedScoped<ITradeStateProvider, BuyerConfirmProvider>();
            sc.AddManagedScoped<ITradeStateProvider, BuyerCompleteProvider>();
            sc.AddManagedScoped<ITradeStateProvider, SellerConfirmProvider>();
            sc.AddManagedScoped<ITradeStateProvider, SellerCompleteProvider>();
            sc.AddManagedScoped<ITradeStateProvider, SellerSettlementProvider>();

            sc.AddDataModules<
				SF.Biz.Trades.DataModels.DataTrade,
                SF.Biz.Trades.DataModels.DataTradeItem
                >(TablePrefix ?? "Biz");

            sc.AddRemindable<TradeRemindable>();

			sc.InitServices("Trade", async (sp, sim, parent) =>
			 {
				 await sim.DefaultService<ITradeManager, TradeManager>(null)
					.WithConsolePages("交易管理/交易管理")
					.Ensure(sp, parent);
                 await sim.DefaultService<ITradeItemManager, TradeItemManager>(null)
                    .WithConsolePages("交易管理/交易明细管理")
                    .Ensure(sp, parent);

                 await sim.DefaultService<IBuyerTradeService, BuyerTradeService>(null)
                   .Ensure(sp, parent);
                 await sim.DefaultService<ISellerTradeService, SellerTradeService>(null)
                   .Ensure(sp, parent);

                 await sim.Service<ITradeStateProvider, BuyerConfirmProvider>(null)
                    .WithIdent(TradeState.BuyerConfirm.ToString())
                    .Ensure(sp, parent);
                 await sim.Service<ITradeStateProvider, BuyerCompleteProvider>(null)
                    .WithIdent(TradeState.BuyerComplete.ToString())
                    .Ensure(sp, parent);
                 await sim.Service<ITradeStateProvider, SellerConfirmProvider>(null)
                    .WithIdent(TradeState.SellerConfirm.ToString())
                    .Ensure(sp, parent);
                 await sim.Service<ITradeStateProvider, SellerCompleteProvider>(null)
                    .WithIdent(TradeState.SellerComplete.ToString())
                    .Ensure(sp, parent);
                 await sim.Service<ITradeStateProvider, SellerSettlementProvider>(null)
                    .WithIdent(TradeState.SellerSettlement.ToString())
                    .Ensure(sp, parent);

             });


			return sc;
		}
	}
}
