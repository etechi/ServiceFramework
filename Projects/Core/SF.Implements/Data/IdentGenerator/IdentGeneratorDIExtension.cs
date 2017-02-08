using SF.Core.DI;
namespace SF.Services.ManagedServices
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
			return sc;
		}
	}
}