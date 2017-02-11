using System;
namespace SF.Core.DI
{
	public static class ManagedServiceCollectionExtension
	{
		public static IDIServiceCollection Normal(this IDIServiceCollection sc)
		{
			var msc = sc as Core.ManagedServices.IManagedServiceCollection;
			if (msc != null) return msc.NormalServiceCollection;
			return sc;
		}
	}
}
