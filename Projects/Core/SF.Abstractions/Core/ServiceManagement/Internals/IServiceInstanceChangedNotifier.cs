namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(string ServiceType,int AppId, long Id);
		void NotifyDefaultChanged(string ServiceType,int AppId);
	}

}
