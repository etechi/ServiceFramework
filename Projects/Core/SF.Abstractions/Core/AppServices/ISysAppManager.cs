using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.AppServices
{
	public class SysAppQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("����ʵ������")]
		public string Name { get; set; }
		
	}

	[EntityManager("Ӧ��")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("ϵͳӦ��", "ϵͳӦ��", GroupName = "ϵͳӦ��")]
	public interface ISysAppManager :
		IEntitySource<long, Models.SysApplication, SysAppQueryArgument>
	{

	}
}
