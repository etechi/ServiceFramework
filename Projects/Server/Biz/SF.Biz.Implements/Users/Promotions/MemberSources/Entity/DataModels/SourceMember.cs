using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Promotions.MemberSources.Entity.DataModels
{
	[Table("UserSourceMember")]
	public class SourceMember<TMemberSource,TSourceMember> : ItemEntityBase<TMemberSource>
		where TMemberSource : MemberSource<TMemberSource,TSourceMember>,new()
		where TSourceMember : SourceMember<TMemberSource, TSourceMember>,new()
	{
	}
}

