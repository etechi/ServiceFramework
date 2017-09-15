using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using SF.Metadata;
using SF.Entities;

namespace SF.Core.ServiceManagement
{
	public static class DataEntityDICollectionExtension
	{
		public static IServiceCollection AddDataEntityProviders(this IServiceCollection sc)
		{
			sc.AddSingleton<IMetadataAttributeValuesProvider<EntityIdentAttribute>, EntityIdentAttributeMetadataValuesProvider>();
			sc.AddSingleton<IMetadataAttributeValuesProvider<EntityManagerAttribute>, EntityManagerAttributeMetadataValuesProvider>();
			sc.AddSingleton(sp =>
			{
				var svcResolver = sp.Resolve<IServiceDeclarationTypeResolver>();
				return new DataEntityConfigCache(
					from type in sc.GetServiceTypes()
					let em = type.GetCustomAttribute<EntityManagerAttribute>(true)
					where em != null
					let el = type.AllInterfaces().FirstOrDefault(ei => ei.IsGeneric() && ei.GetGenericTypeDefinition() == typeof(IEntityLoadable<,>))
					where el != null
					let targs = el.GetGenericArguments()
					select (DataEntityConfigItem)Activator.CreateInstance(
						typeof(DataEntityConfigItem<,>).MakeGenericType(
						targs[0],
						targs[1]
						),
						svcResolver.GetTypeIdent(type)
						)
					);
			});

			sc.AddScoped<IDataEntityResolver, DefaultDataEntityResolver>();

			sc.Add(typeof(IDataSetEntityManager<>), typeof(DataSetEntityManager<>),ServiceImplementLifetime.Scoped);
			
			return sc;
		}
	}
}
