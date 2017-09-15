using SF.Auth;
using SF.Data.Entity;
using SF.Metadata;

namespace SF.Core.AppServices
{
	public class SysAppQueryArgument : IQueryArgument<long>
	{
		[Comment("ID")]
		public Option<long> Id { get; set; }

		[Comment("服务实现名称")]
		public string Name { get; set; }
		
	}

	[EntityManager("应用")]
	[Authorize("sysadmin")]
	[NetworkService]
	[Comment("系统应用", "系统应用", GroupName = "系统应用")]
	public interface ISysAppManager :
		IEntitySource<long, Models.SysApplication, SysAppQueryArgument>
	{

	}
}
