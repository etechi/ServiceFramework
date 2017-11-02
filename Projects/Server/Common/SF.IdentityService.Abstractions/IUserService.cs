#region Apache License Version 2.0
/*----------------------------------------------------------------
Copyright 2017 Yang Chen (cy2000@gmail.com)

Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file
except in compliance with the License. You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the
License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND,
either express or implied. See the License for the specific language governing permissions
and limitations under the License.
Detail: https://github.com/etechi/ServiceFramework/blob/master/license.md
----------------------------------------------------------------*/
#endregion Apache License Version 2.0

using SF.Auth.IdentityServices.Models;
using SF.Metadata;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices
{
	public class SigninArgument
	{
		[Comment("用户")]
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
		[Comment("身份验证服务")]
		public string CredentialProvider { get; set; }

		[Comment("人工操作验证码")]
		public string CaptchaCode { get; set; }

		[Comment("用户")]
		public string Credential { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		[Comment("身份验证服务")]
		public string CredentialProvider { get; set; }

		[Comment("用户")]
		public string Credential { get; set; }

		[Comment("验证码")]
		public string VerifyCode { get; set; }

		[Comment("新密码")]
		public string NewPassword { get; set; }

		[Comment("是否返回身份令牌")]
		public bool ReturnToken { get; set; }
	}
	public class SignupArgument
	{
		[Comment("身份验证服务ID")]
		public string CredentialProvider { get; set; }

		[Comment("身份信息")]
		public User User { get; set; }

		[Comment("用户")]
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

		public string[] Roles { get; set; }

		[Comment("附加参数")]
		public Dictionary<string,string> ExtraArgument{get;set;}
	}

	public class SendCreateIdentityVerifyCodeArgument
	{
		[Comment("身份验证服务")]
		public string CredentialProvider { get; set; }

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
	[Comment("用户服务")]
	public interface IUserService
    {
		[Comment("获取当前用户ID")]
		[Authorize]
		Task<long?> GetCurUserId();

		[Comment("获取当前用户")]
		Task<User> GetCurUser();

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


		[Comment("修改用户信息")]
		Task Update(User User);

		[Comment("从访问令牌提取身份ID")]
		Task<long> ValidateAccessToken(string AccessToken);

		[Comment("发送用户创建验证信息")]
		Task<string> SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg);

		[Comment("注册用户")]
		Task<string> Signup(SignupArgument Arg, bool VerifyCode);

		[Comment("根据用户ID获取身份信息")]
		Task<User> GetUser(long Id);
	}

}

