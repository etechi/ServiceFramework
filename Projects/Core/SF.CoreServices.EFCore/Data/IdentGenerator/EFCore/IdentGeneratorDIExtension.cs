using SF.DI;

namespace SF.Services.Management
{
	public static class IdentGeneratorDIExtension
	{
		public static Services.Management.IManagedServiceCollection UseEFCoreIdentGenerator(
			this Services.Management.IManagedServiceCollection msc,
			string TablePrefix
			)
		{
			msc.NormalServiceCollection.UseDataModules<SF.Data.IdentGenerator.EFCore.DataModels.IdentSeed>(TablePrefix);
			msc.AddScoped<SF.Data.IIdentGenerator, SF.Data.IdentGenerator.EFCore.IdentGenerator>();
			return msc;
		}
	}
}