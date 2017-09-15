using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Promotions.MemberSources.Entity.DataModels
{
	[Table("UserMemberSource")]
	public class MemberSource<TMemberSource,TSourceMember> : TreeContainerEntityBase<TMemberSource,TSourceMember>
		where TMemberSource : MemberSource<TMemberSource, TSourceMember>,new()
		where TSourceMember : SourceMember<TMemberSource, TSourceMember>,new()
	{
	}
}

