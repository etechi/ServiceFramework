using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.AppServices
{
	public class SysFeatureQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("功能名称")]
		public string Name { get; set; }

	}

	[EntityManager("系统功能")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("系统功能", "系统功能", GroupName = "系统功能管理")]
	public interface ISysFeatureManager :
		IEntityManager<long, Models.SysFeature>,
		IEntitySource<long, Models.SysFeature, SysFeatureQueryArgument>
	{

	}
}
