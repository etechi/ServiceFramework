using SF.Core.DI;
using SF.Data.Entity;
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
			sc.AddScoped<SF.Data.Entity.IDataContext, SF.Data.Entity.DataContext>();
			sc.Add(typeof(SF.Data.Entity.IDataSet<>),typeof(SF.Data.Entity.DataSet<>),ServiceLifetime.Scoped);
			return sc;
		}
	}
	

}
