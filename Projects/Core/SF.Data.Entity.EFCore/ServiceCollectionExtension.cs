using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using SF.Data.Entity;
using SF.Data.Entity.EntityFrameworkCore;
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
			sc.AddScoped<IDataContextProviderFactory>(x => new DataContextProviderFactory<TDbContext>(()=>x.GetRequiredService<TDbContext>()));
			return sc;
		}
	}
	

}
