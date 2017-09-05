using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data;
using SF.Data.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using System.Data.Common;

namespace SF.Core.ServiceManagement
{
	public static class EFCoreServiceCollectioExtension
	{
		
		public static IServiceCollection AddEFCoreDataEntity<TDbContext>(this IServiceCollection sc)
			where TDbContext : DbContext
		{
			sc.AddScoped<IDataContextProviderFactory>(x => 
				new DataContextProviderFactory<TDbContext>(
					()=>x.GetRequiredService<TDbContext>()
					)
				);
			return sc;
		}
		public static IServiceCollection AddEFCoreDataEntity(this IServiceCollection sc,Func<IServiceProvider,DbConnection,DbContext> DbContextCreator)
		{
			sc.AddScoped<IDataContextProviderFactory>(x =>
				new DataContextProviderFactory(
					() => DbContextCreator(x,x.GetRequiredService<DbConnection>())
					)
				);
			return sc;
		}
	}
	

}
