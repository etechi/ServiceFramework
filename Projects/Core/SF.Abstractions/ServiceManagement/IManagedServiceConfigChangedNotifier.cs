namespace SF.ServiceManagement
{
	public interface IManagedServiceConfigChangedNotifier
	{
		void NotifyChanged(string Id);
		void NotifyDefaultChanged(string Type);
	}

}
