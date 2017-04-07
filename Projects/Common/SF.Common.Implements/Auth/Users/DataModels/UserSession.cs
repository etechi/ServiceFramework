using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;
using SF.Clients;

namespace SF.Auth.Users.DataModels
{
	[Table("SysUserSession")]
	public class UserSession: IObjectWithId<long>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Comment("ID")]
		public long Id { get; set; }


		[Index]
		[Comment("用户ID")]
		public long UserId { get; set; }

		[Index]
		[Comment("创建时间")]
		public virtual DateTime CreatedTime { get; set; }

		[Index]
		[Comment("最后活动时间")]
		public virtual DateTime LastActiveTime { get; set; }

		[Comment("客户端设备类型")]
		public ClientDeviceType ClientType { get; set; }

		[MaxLength(100)]
		[Comment("客户端地址")]
		public string ClientAddress { get; set; }

		[Comment("客户端代理")]
		public string ClientAgent { get; set; }

		[Index]
		[Comment("用户类型")]
		public UserType UserType { get; set; }

	}
}
