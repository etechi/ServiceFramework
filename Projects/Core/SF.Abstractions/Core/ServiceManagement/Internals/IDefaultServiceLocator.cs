namespace SF.Core.ServiceManagement.Internals
{
	[UnmanagedService]
	public interface IDefaultServiceLocator
	{
		string Locate(string Type,int AppId);
	}

}
