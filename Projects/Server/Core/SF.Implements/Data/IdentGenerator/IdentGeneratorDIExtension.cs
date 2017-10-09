using SF.Core.ServiceManagement.Management;
namespace SF
{
	class IdentScope { }
}

namespace SF.Core.ServiceManagement
{
	public static class IdentGeneratorDIExtension
	{
		public static IServiceCollection AddIdentGenerator(
			this IServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.AddDataModules<SF.Data.IdentGenerator.DataModels.IdentSeed>(TablePrefix);
			sc.Add(typeof(SF.Data.IIdentGenerator<>),typeof(SF.Data.IdentGenerator.StorageIdentGenerator<>),ServiceImplementLifetime.Scoped);
			sc.AddScoped<SF.Data.IIdentGenerator, SF.Data.IdentGenerator.StorageIdentGenerator>();
			return sc;
		}
	}
}