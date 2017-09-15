namespace SF.Core.ServiceManagement.Internals
{
	public class ServiceInstanceChanged
	{
		public long Id { get; set; }
	}
	public class InternalServiceChanged
	{
		public long? ScopeId { get; set; }
		public string ServiceType { get; set; }
	}
}
