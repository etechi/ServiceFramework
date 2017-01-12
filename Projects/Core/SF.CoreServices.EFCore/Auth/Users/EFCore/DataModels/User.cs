using ServiceProtocol.Data.Entity;
using SF.Data;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.Users.EFCore.DataModels
{
	[Table("AuthUser")]
	public class User : IObjectWithId<long>
	{
		[Key]
		[Display(Name ="类型")]
		[DatabaseGenerated(DatabaseGeneratedOption.None)]
		public virtual long Id { get; set; }

		[MaxLength(100)]
		[Display(Name = "用户名")]
		public virtual string UserName { get; set; }

		[MaxLength(200)]
		[Display(Name = "邮件")]
		public virtual string Email { get; set; }

		[MaxLength(50)]
		[Display(Name = "手机")]
		public virtual string PhoneNumber { get; set; }
	
		[MaxLength(100)]
		[Display(Name = "昵称")]
		public virtual string NickName { get; set; }

		[MaxLength(200)]
		[Display(Name = "图片")]
		public virtual string Image { get; set; }

		[MaxLength(200)]
		[Display(Name = "图标")]
		public virtual string Icon { get; set; }

		[Display(Name = "性别")]
		public virtual SexType Sex { get; set; }

		[MaxLength(100)]
		[Display(Name = "密码哈希")]
		public virtual string PasswordHash { get; set; }


		[Display(Name = "锁定超时")]
		public virtual DateTime? LockoutEndDateUtc { get; set; }

		[Display(Name = "登陆失败次数")]
		public virtual int AccessFailedCount { get; set; }

		[Display(Name = "逻辑状态")]
		public virtual LogicObjectState ObjectState { get; set; }

		[Display(Name = "邀请人")]
		public virtual long? InviterId { get; set; }

		[MaxLength(100)]
		[Required]
		[Display(Name = "安全标识")]
		public virtual string SecurityStamp { get; set; }

		[Index]
		[Display(Name = "创建时间")]
		public virtual DateTime CreatedTime { get; set; }

		[Display(Name = "更新时间")]
		public virtual DateTime UpdatedTime { get; set; }

		[Display(Name = "登录次数")]
		public virtual int SigninCount { get; set; }

		[Index]
		[Display(Name = "最后登录时间")]
		public virtual DateTime? LastSigninTime { get; set; }


		[Display(Name = "注册设备")]
		public virtual ClientDeviceType SignupDeviceType { get; set; }

		[MaxLength(100)]
		[Display(Name = "注册地址")]
		public string SignupAddress { get; set; }

		[Display(Name = "最后登录设备")]
		public virtual ClientDeviceType LastDeviceType { get; set; }

		[MaxLength(100)]
		[Display(Name = "最后登录地址")]
		public virtual string LastAddress { get; set; }

		[Index]
		[Display(Name = "用户类型")]
		public virtual UserType UserType { get; set; }

		[Index]
		[Display(Name = "是否无标识")]
		public virtual bool NoIdents { get; set; }

		[Index(Order = 1)]
		[MaxLength(50)]
		[Display(Name = "注册标识提供者")]
		public virtual string SignupIdentProvider { get; set; }

		[MaxLength(200)]
		[Index(Order = 2)]
		[Display(Name = "注册标识值")]
		public virtual string SignupIdentValue { get; set; }


		[Display(Name = "乐观锁时间戳")]
		[ConcurrencyCheck]
		[Timestamp]
		public virtual byte[] TimeStamp { get; set; }
	}
}
