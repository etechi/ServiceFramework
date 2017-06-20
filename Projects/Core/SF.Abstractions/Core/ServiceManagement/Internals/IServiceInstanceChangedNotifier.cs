namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(string ServiceType,int AppId,string Id);
		void NotifyDefaultChanged(string ServiceType,int AppId);
	}

}
