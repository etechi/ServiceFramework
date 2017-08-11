using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Data.Entity;

namespace SF.Core.ServiceManagement
{
	public static class DataEntityResolverDICollectionExtension
	{
		public static IServiceCollection AddDataEntity(this IServiceCollection sc)
		{
			sc.AddSingleton(sp =>
				new DataEntityConfigCache(
					from type in sc.GetServiceTypes()
					let em = type.GetCustomAttribute<EntityManagerAttribute>(true)
					where em!=null
					let el =type.GetInterfaces().FirstOrDefault(ei=>ei.IsGeneric() && ei.GetGenericTypeDefinition()==typeof(IEntityLoader<,>))
					where el!=null
					let targs= el.GetGenericArguments()
					select (DataEntityConfigItem)Activator.CreateInstance(
						typeof(DataEntityConfigItem<,>).MakeGenericType(
						targs[0],
						targs[1]
						),
						em.Entity
						)
					));

			sc.AddScoped<IDataEntityResolver, DefaultDataEntityResolver>();
			return sc;
		}
	}
}
