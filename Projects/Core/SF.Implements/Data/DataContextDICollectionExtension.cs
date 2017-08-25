using SF.Core.DI;
using SF.Data;
using SF.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
namespace SF.Core.ServiceManagement
{
	
	public static class DataContextCollectionExtension
	{
		public static IServiceCollection AddDataContext(this IServiceCollection sc, DataSourceConfig Config)
		{
			sc.AddSingleton<IDataSource>(new DefaultDataSource(Config));
			sc.AddScoped(sp => sp.Resolve<IDataSource>().Connect());

			sc.AddScoped<SF.Data.IDataContext, SF.Data.DataContext>();
			sc.Add(typeof(SF.Data.IDataSet<>),typeof(SF.Data.DataSet<>),ServiceImplementLifetime.Scoped);
			sc.AddScoped<SF.Data.ITransactionScopeManager, TransactionScopeManager>();
			return sc;
		}
	}
	

}
