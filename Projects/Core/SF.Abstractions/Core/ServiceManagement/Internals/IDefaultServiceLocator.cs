namespace SF.Core.ServiceManagement.Internals
{
	public interface IDefaultServiceLocator
	{
		long? Locate(string Type);
	}

}
