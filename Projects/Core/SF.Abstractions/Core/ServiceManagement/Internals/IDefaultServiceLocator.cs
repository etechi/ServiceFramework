namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IDefaultServiceLocator
	{
		long? Locate(string Type);
	}

}
