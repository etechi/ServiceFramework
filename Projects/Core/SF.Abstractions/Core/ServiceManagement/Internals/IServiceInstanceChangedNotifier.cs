namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(long Id);
		void NotifyInternalServiceChanged( long? ScopeId,string ServiceType);
	}

}
