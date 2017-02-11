namespace SF.Core.ManagedServices.Runtime
{
	public interface IManagedServiceConfigChangedNotifier
	{
		void NotifyChanged(string Id);
		void NotifyDefaultChanged(string Type);
	}

}
