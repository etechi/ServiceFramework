using SF.Metadata;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInstanceMeta
	{
		[Comment("应用ID")]
		int AppId { get; }

		long DataScopeId { get; }

		long Id { get; }
	}

}
