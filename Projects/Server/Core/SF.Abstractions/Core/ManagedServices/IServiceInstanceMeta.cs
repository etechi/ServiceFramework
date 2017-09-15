using SF.Metadata;

namespace SF.Core.ManagedServices
{
	public interface IServiceInstanceMeta
	{
		[Comment("应用ID")]
		long AppId { get; }

		[Comment("功能ID")]
		long FeatureId { get; }

		string Ident { get; }
	}

}
