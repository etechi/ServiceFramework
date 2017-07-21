using SF.Data.Storage;
using SF.Data.Storage.EF6;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Core.ServiceManagement
{
	
	public static class DICollectioExtension
	{
		

		public static IServiceCollection UseEF6DataEntity<TDbContext>(this IServiceCollection sc)
			where TDbContext : SF.Data.Storage.DbContext
		{
			sc.AddScoped<IDataContextProviderFactory>(x =>
				new DataContextProviderFactory<TDbContext>(() => x.Resolve<TDbContext>())
				);
			return sc;
		}
		public static IServiceCollection UseEF6DataEntity(this IServiceCollection sc)
		{
			sc.AddScoped<IDataContextProviderFactory, DataContextProviderFactory>();
			return sc;
		}
	}
	

}
