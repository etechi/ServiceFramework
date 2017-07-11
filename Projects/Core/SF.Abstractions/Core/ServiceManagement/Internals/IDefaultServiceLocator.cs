namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IDefaultServiceLocator
	{
		long? Locate(long? ScopeId,string ServiceType);
	}

}
