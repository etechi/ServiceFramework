using SF.Core.ServiceManagement;
namespace SF.Core.Hosting
{
	public static class AppInstanceBuilderExtension
	{
		public static IAppInstance Build(this AppInstanceBuilder Builder)
		{
			return Builder.Build(sc => sc.BuildServiceResolver());
		}
	}

}
