using SF.Data.Storage;
using SF.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SF.Metadata;

namespace SF.Auth.Users.DataModels
{
	[Table("SysUser")]
	public class User: IObjectWithId<long>
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Comment("ID")]
		public long Id { get; set; }

		[MaxLength(100)]
		[Comment("用户名")]
		public virtual string UserName { get; set; }

		[MaxLength(200)]
		[Comment("邮件")]
		public virtual string Email { get; set; }

		[MaxLength(50)]
		[Comment("手机")]
		public virtual string PhoneNumber { get; set; }

		[MaxLength(100)]
		[Comment("昵称")]
		public virtual string NickName { get; set; }

		[MaxLength(200)]
		[Comment("图片")]
		public virtual string Image { get; set; }

		[MaxLength(200)]
		[Comment("图标")]
		public virtual string Icon { get; set; }

		[Comment("性别")]
		public virtual SexType Sex { get; set; }

		[MaxLength(100)]
		[Comment("密码哈希")]
		public virtual string PasswordHash { get; set; }

		[Comment("逻辑状态")]
		public virtual LogicObjectState ObjectState { get; set; }

		[MaxLength(100)]
		[Required]
		[Comment("安全标识")]
		public virtual string SecurityStamp { get; set; }

		[Index]
		[Comment("创建时间")]
		public virtual DateTime CreatedTime { get; set; }
		[Comment("更新时间")]
		public virtual DateTime UpdatedTime { get; set; }
		[Comment("登录次数")]
		public virtual int SigninCount { get; set; }
		[Index]
		[Comment("最后登录时间")]
		public virtual DateTime? LastSigninTime { get; set; }


		[Comment("注册设备")]
		public string SignupDeviceType { get; set; }

		[MaxLength(100)]
		[Comment("注册地址")]
		public string SignupAddress { get; set; }

		[Comment("最后登录设备")]
		public string LastDeviceType { get; set; }

		[MaxLength(100)]
		[Comment("最后登录地址")]
		public string LastAddress { get; set; }

		//[Index]
		//[Comment("是否是管理员")]
		//public bool IsAdmin { get; set; }

		[Index]
		[Comment("用户类型")]
		public UserType UserType { get; set; }

		[Index]
		[Comment("是否无标识")]
		public bool NoIdents { get; set; }

		[Index(Order = 1)]
		[Comment("注册标识类型")]
		public int SignupIdentType { get; set; }

		[MaxLength(200)]
		[Index(Order = 2)]
		[Comment("注册标识值")]
		public string SignupIdentValue { get; set; }

		[Display(Name = "锁定超时")]
		public virtual DateTime? LockoutEndDate { get; set; }

		[Display(Name = "登陆失败次数")]
		public virtual int AccessFailedCount { get; set; }

	}
}
