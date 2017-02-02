using SF.Core.DI;
using SF.Data.Entity;
using SF.Data.Entity.EF6;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SF.Core.DI
{
	
	public static class DICollectioExtension
	{
		

		public static IDIServiceCollection UseEF6DataEntity<TDbContext>(this IDIServiceCollection sc)
			where TDbContext : SF.Data.Entity.DbContext
		{
			sc.AddScoped<IDataContextProviderFactory>(x =>
				new DataContextProviderFactory<TDbContext>(()=>x.Resolve<TDbContext>())
				);
			return sc;
		}
	}
	

}
