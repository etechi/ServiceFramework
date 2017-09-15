using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.AppServices
{
	public class SysFeatureQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("��������")]
		public string Name { get; set; }

	}

	[EntityManager("ϵͳ����")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("ϵͳ����", "ϵͳ����", GroupName = "ϵͳ���ܹ���")]
	public interface ISysFeatureManager :
		IEntityManager<long, Models.SysFeature>,
		IEntitySource<long, Models.SysFeature, SysFeatureQueryArgument>
	{

	}
}
