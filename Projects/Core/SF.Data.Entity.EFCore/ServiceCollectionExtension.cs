using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ServiceProtocol.Data.Entity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
namespace SF.DI
{
	
	public static class EFCoreServiceCollectioExtension
	{
		
		public static IDIServiceCollection UseEFCoreDataEntity<TDbContext>(this IDIServiceCollection sc)
			where TDbContext : DbContext
		{
			sc.AddScoped<SF.Data.Entity.IDataContext, SF.Data.Entity.DataContext>();
			sc.AddTransient<SF.Data.Entity.IDataContextProvider, SF.Data.Entity.EntityFrameworkCore.EntityDbContextProvider<TDbContext>>();
			sc.AddTransient<Func<SF.Data.Entity.IDataContextProvider>>(x => () => x.GetRequiredService<SF.Data.Entity.IDataContextProvider>());
			return sc;
		}
	}
	

}
