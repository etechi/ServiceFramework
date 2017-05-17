using SF.Metadata;

namespace SF.Core.ServiceManagement
{
	public interface IServiceInstanceMeta
	{
		[Comment("应用ID")]
		long AppId { get; }

		long Id { get; }
	}

}
