using System;
namespace SF.ServiceManagement
{
	public static class ManagedServiceCollectionExtension
	{
		public static IManagedServiceCollection Add(this IManagedServiceCollection msc, Type ServiceType, Type ImplementType)
		{
			msc.Add(new ManagedServiceDescriptor(ServiceType, ImplementType));
			return msc;
		}
		public static IManagedServiceCollection AddScoped<I, T>(this IManagedServiceCollection msc)
			where I : class
			where T : I
			=>
			msc.Add(typeof(I), typeof(T));
	}
}
