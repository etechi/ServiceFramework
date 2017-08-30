using SF.Auth.Identities.Models;
using SF.Metadata;
using System.Threading.Tasks;

namespace SF.Auth.Identities
{
	public class SigninArgument
	{
		[Comment("身份标识")]
		public string Credential { get; set; }

		[Comment("密码")]
		public string Password { get; set; }

		[Comment("过期时间")]
		public int? Expires { get; set; }

		[Comment("人工操作验证码")]
		public string CaptchaCode { get; set; }

		[Comment("是否返回身份令牌")]
		public bool ReturnToken { get; set; }
		
	}
	

	public class SendPasswordRecorveryCodeArgument
	{
		[Comment("身份验证服务ID")]
		public long? CredentialProviderId { get; set; }

		[Comment("人工操作验证码")]
		public string CaptchaCode { get; set; }

		[Comment("身份标识")]
		public string Credential { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		[Comment("身份验证服务ID")]
		public long? CredentialProviderId { get; set; }

		[Comment("身份标识")]
		public string Credential { get; set; }

		[Comment("验证码")]
		public string VerifyCode { get; set; }

		[Comment("新密码")]
		public string NewPassword { get; set; }

		[Comment("是否返回身份令牌")]
		public bool ReturnToken { get; set; }
	}
	public class CreateIdentityArgument
	{
		[Comment("身份验证服务ID")]
		public long? CredentialProviderId { get; set; }

		[Comment("身份信息")]
		public Identity Identity { get; set; }

		[Comment("身份标识")]
		public string Credential { get; set; }

		[Comment("密码")]
		public string Password { get; set; }

		[Comment("人工操作验证码")]
		public string CaptchaCode { get; set; }

		[Comment("验证码")]
		public string VerifyCode { get; set; }

		[Comment("是否返回身份令牌")]
		public bool ReturnToken { get; set; }

		[Comment("过期时间")]
		public int? Expires { get; set; }
	}

	public class SendCreateIdentityVerifyCodeArgument
	{
		[Comment("身份验证服务ID")]
		public long? CredentialProviderId { get; set; }

		[Comment("人工操作验证码")]
		public string Credetial { get; set; }

		[Comment("人工操作验证码")]
		public string CaptchaCode { get; set; }
	}

	public class SetPasswordArgument
	{
		[Comment("就密码")]
		public string OldPassword { get; set; }

		[Comment("新密码")]
		public string NewPassword { get; set; }

		[Comment("是否返回身份令牌")]
		public bool ReturnToken { get; set; }
	}


	[NetworkService]
	[Comment("身份标识服务")]
	public interface IIdentityService
    {
		[Comment("获取当前身份标识ID")]
		Task<long?> GetCurIdentityId();

		[Comment("获取当前身份标识")]
		Task<Identity> GetCurIdentity();

		[Comment("登录")]
		Task<string> Signin(SigninArgument Arg);

		[Comment("注销")]
		Task Signout();

		[Comment("发送忘记密码验证消息")]
		Task<string> SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);

		[Comment("使用验证消息重置密码")]
		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		[Comment("设置密码")]
		Task<string> SetPassword(SetPasswordArgument Arg);


		[Comment("修改身份标识信息")]
		Task UpdateIdentity(Identity Identity);

		[Comment("从访问令牌提取身份ID")]
		Task<long> ParseAccessToken(string AccessToken);

		[Comment("发送身份标识创建验证信息")]
		Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg);

		[Comment("创建身份标识")]
		Task<string> CreateIdentity(CreateIdentityArgument Arg, bool VerifyCode);

		[Comment("根据身份标识ID获取身份信息")]
		Task<Identity> GetIdentity(long Id);
	}

}

