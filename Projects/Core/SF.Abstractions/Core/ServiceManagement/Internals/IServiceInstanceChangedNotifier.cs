namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(string ServiceType,long Id);
		void NotifyDefaultChanged(string ServiceType);
	}

}
