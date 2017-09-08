using SF.Entities.DataModels;
using SF.Metadata;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Users.Promotions.MemberInvitations.Entity.DataModels
{
	[Table("UserMemberInvatation")]
	public class MemberInvatation<TMemberInvatation> : TreeNodeEntityBase<TMemberInvatation>
		where TMemberInvatation : MemberInvatation<TMemberInvatation>, new()
	{
	}
}

