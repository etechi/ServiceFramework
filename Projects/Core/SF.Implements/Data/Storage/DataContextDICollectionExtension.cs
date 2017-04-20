using SF.Core.DI;
using SF.Data.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SF.Core.DI
{
	
	public static class DataContextCollectionExtension
	{
		public static IDIServiceCollection UseDataContext(this IDIServiceCollection sc)
		{
			sc.AddScoped<SF.Data.Storage.IDataContext, SF.Data.Storage.DataContext>();
			sc.Add(typeof(SF.Data.Storage.IDataSet<>),typeof(SF.Data.Storage.DataSet<>),ServiceLifetime.Scoped);
			sc.AddScoped<SF.Data.Storage.ITransactionScopeManager, TransactionScopeManager>();
			return sc;
		}
	}
	

}
