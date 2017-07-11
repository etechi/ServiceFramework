namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(long Id);
		void NotifyDefaultChanged(long? ScopeId,string ServiceType);
	}

}
