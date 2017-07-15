namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(long Id);
		void NotifyInternalServiceChanged( long? ScopeId,string ServiceType);
	}

}
