using SF.Core.DI;
using SF.Core.ServiceManagement.Management;

namespace SF.Core.ServiceManagement
{
	public static class IdentGeneratorDIExtension
	{
		public static IServiceCollection UseIdentGenerator(
			this IServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.UseDataModules<SF.Data.IdentGenerator.DataModels.IdentSeed>(TablePrefix);
			sc.AddScoped<SF.Data.IIdentGenerator, SF.Data.IdentGenerator.StorageIdentGenerator>();
			sc.AddInitializer(
				"初始化对象标识生成器",
				async sp =>
				{
					await sp.Resolve<IServiceInstanceManager>().EnsureDefaultService<
						SF.Data.IIdentGenerator,
						SF.Data.IdentGenerator.StorageIdentGenerator
						>(
						new
						{
							Setting = new Data.IdentGenerator.IdentGeneratorSetting()
						});
				});
			return sc;
		}
	}
}