using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data.Storage;
using SF.Data.Storage.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SF.Core.DI;
using System.Data.Common;

namespace SF.Core.DI
{
	public static class EFCoreServiceCollectioExtension
	{
		
		public static IServiceCollection UseEFCoreDataEntity<TDbContext>(this IServiceCollection sc)
			where TDbContext : DbContext
		{
			sc.AddScoped<IDataContextProviderFactory>(x => 
				new DataContextProviderFactory<TDbContext>(
					()=>x.GetRequiredService<TDbContext>()
					)
				);
			return sc;
		}
		public static IServiceCollection UseEFCoreDataEntity(this IServiceCollection sc,Func<DbConnection,DbContext> DbContextCreator)
		{
			sc.AddScoped<IDataContextProviderFactory>(x =>
				new DataContextProviderFactory(
					() => DbContextCreator(x.GetRequiredService<DbConnection>())
					)
				);
			return sc;
		}
	}
	

}
