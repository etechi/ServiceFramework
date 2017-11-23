using SF.Sys.Entities.DataModels;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Biz.MemberInvitations.Entity.DataModels
{
	[Table("UserMemberInvatation")]
	public class MemberInvatation<TMemberInvatation> : TreeNodeEntityBase<TMemberInvatation>
		where TMemberInvatation : MemberInvatation<TMemberInvatation>, new()
	{
	}
}

