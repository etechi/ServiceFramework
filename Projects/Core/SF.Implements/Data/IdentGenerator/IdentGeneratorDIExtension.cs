using SF.Core.DI;
using SF.Core.ServiceManagement.Management;

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
			sc.AddScoped<SF.Data.IIdentGenerator, SF.Data.IdentGenerator.StorageIdentGenerator>();
			return sc;
		}
	}
}