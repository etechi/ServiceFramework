namespace SF.Core.ServiceManagement.Internals
{
	public interface IServiceInstanceConfigChangedNotifier
	{
		void NotifyChanged(string ServiceType,long Id);
		void NotifyDefaultChanged(string ServiceType);
	}

}
