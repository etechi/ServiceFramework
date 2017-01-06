namespace SF.Services.Management
{
	public interface IManagedServiceConfigChangedNotifier
	{
		void NotifyChanged(string Id);
		void NotifyDefaultChanged(string Type);
	}

}
