using SF.Core.DI;
using SF.Data;
using SF.Data.Storage;
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
		public static IServiceCollection UseDataContext(this IServiceCollection sc)
		{
			sc.AddTransient<IDataSource, DefaultDataSource>();
			sc.AddScoped<DbConnection>(sp => sp.Resolve<IDataSource>().Connect());

			sc.AddScoped<SF.Data.Storage.IDataContext, SF.Data.Storage.DataContext>();
			sc.Add(typeof(SF.Data.Storage.IDataSet<>),typeof(SF.Data.Storage.DataSet<>),ServiceImplementLifetime.Scoped);
			sc.AddScoped<SF.Data.Storage.ITransactionScopeManager, TransactionScopeManager>();
			return sc;
		}
	}
	

}
