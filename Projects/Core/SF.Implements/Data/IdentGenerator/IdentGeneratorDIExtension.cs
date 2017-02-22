using SF.Core.DI;
using SF.Core.ManagedServices.Admin;

namespace SF.Core.ManagedServices
{
	public static class IdentGeneratorDIExtension
	{
		public static IDIServiceCollection UseIdentGenerator(
			this IDIServiceCollection sc,
			string TablePrefix=null
			)
		{
			sc.Normal().UseDataModules<SF.Data.IdentGenerator.DataModels.IdentSeed>(TablePrefix);
			sc.AddScoped<SF.Data.IIdentGenerator, SF.Data.IdentGenerator.StorageIdentGenerator>();
			if(sc.IsManagedServiceCollection())
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