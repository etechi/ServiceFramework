using System;
using System.Linq;
using System.Threading.Tasks;
using SF.Data.Storage;
using SF.Data.Entity;
using System.Reflection;
namespace SF.Core.ServiceManagement
{
	public static class EntityServiceDIServiceCollectionExtension
	{
		public static IServiceCollection AddEntityService<I, T, TKey, TEntity>(this IServiceCollection sc)
			where I:class
			where T:I
		{
			sc.AddScoped<I, T>();
			var nsc = sc;
			if (typeof(IEntityLoader<TKey, TEntity>).IsAssignableFrom(typeof(T)))
				nsc.AddScoped(sp => (IEntityLoader<TKey, TEntity>)(object)sp.Resolve<I>());
			if (typeof(IEntityBatchLoader<TKey, TEntity>).IsAssignableFrom(typeof(T)))
				nsc.AddScoped(sp => (IEntityBatchLoader<TKey, TEntity>)(object)sp.Resolve<I>());
			return sc;
		}
	}
   
}