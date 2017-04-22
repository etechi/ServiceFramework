using SF.Data;
using SF.Metadata;
using System.ComponentModel.DataAnnotations;

namespace SF.Users.Members.Models
{
	public class MemberEditable : MemberInternal
	{
		[Comment("图标")]
		[Image]
		public virtual string Icon { get; set; }


		[Comment("密码","新建时必须填写，修改时填写密码将修改会员密码")]
		[Password]
		[MaxLength(100)]
		public virtual string Password { get; set; }


		[Ignore]
		public virtual bool ReturnToken { get; set; }

		[Ignore]
		public string CaptchaCode { get; set; }

		[Ignore]
		public string VerifyCode { get; set; }
	}
}

