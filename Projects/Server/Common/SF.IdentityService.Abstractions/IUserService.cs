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

using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices
{
	public enum SigninMode
	{
		/// <summary>
		/// 仅验证
		/// </summary>
		Validate,
		/// <summary>
		/// Cookie登录
		/// </summary>
		Cookie,
		/// <summary>
		/// 返回AccessToken
		/// </summary>
		AccessToken
	}
	public class SigninArgument
	{
		/// <summary>
		/// 用户
		/// </summary>
		public string Ident { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public int? Expires { get; set; }

		/// <summary>
		/// 人工操作验证码
		/// </summary>
		public string CaptchaCode { get; set; }

		/// <summary>
		/// 验证方式
		/// </summary>
		public SigninMode Mode { get; set; }
		
		/// <summary>
		/// 客户端ID
		/// </summary>
		public string ClientId { get; set; }
	}
	

	public class SendPasswordRecorveryCodeArgument
	{
		/// <summary>
		/// 身份验证服务
		/// </summary>
		public string CredentialProvider { get; set; }

		/// <summary>
		/// 人工操作验证码
		/// </summary>
		public string CaptchaCode { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		public string Credential { get; set; }
	}
	public class ResetPasswordByRecorveryCodeArgument
	{
		/// <summary>
		/// 身份验证服务
		/// </summary>
		public string CredentialProvider { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		public string Credential { get; set; }

		/// <summary>
		/// 验证码
		/// </summary>
		public string VerifyCode { get; set; }

		/// <summary>
		/// 新密码
		/// </summary>
		public string NewPassword { get; set; }

		/// <summary>
		/// 客户端ID
		/// </summary>
		public string ClientId { get; set; }

		
		/// <summary>
		/// 是否返回身份令牌
		/// </summary>
		public bool ReturnToken { get; set; }
	}
	public class SignupArgument
	{
		/// <summary>
		/// 身份验证服务ID
		/// </summary>
		public string CredentialProvider { get; set; }

		/// <summary>
		/// 身份信息
		/// </summary>
		public User User { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		public string Credential { get; set; }

		/// <summary>
		/// 密码
		/// </summary>
		public string Password { get; set; }

		/// <summary>
		/// 人工操作验证码
		/// </summary>
		public string CaptchaCode { get; set; }

		/// <summary>
		/// 验证码
		/// </summary>
		public string VerifyCode { get; set; }

		/// <summary>
		/// 是否返回身份令牌
		/// </summary>
		public bool ReturnToken { get; set; }
		/// <summary>
		/// 客户端ID
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// 过期时间
		/// </summary>
		public int? Expires { get; set; }

		public string[] Roles { get; set; }

		/// <summary>
		/// 附加参数
		/// </summary>
		public Dictionary<string,string> ExtraArgument{get;set;}
	}
	public class BindCredentialArgument
	{
		/// <summary>
		/// 身份验证服务ID
		/// </summary>
		public string CredentialProvider { get; set; }

		/// <summary>
		/// 用户
		/// </summary>
		public string Credential { get; set; }

		/// <summary>
		/// 人工操作验证码
		/// </summary>
		public string CaptchaCode { get; set; }

		/// <summary>
		/// 验证码
		/// </summary>
		public string VerifyCode { get; set; }

	}

	public class SendBindCredentialVerifyCodeArgument
	{
		/// <summary>
		/// 身份验证服务
		/// </summary>
		public string CredentialProvider { get; set; }

		/// <summary>
		/// 登录凭证
		/// </summary>
		public string Credetial { get; set; }

		/// <summary>
		/// 人工操作验证码
		/// </summary>
		public string CaptchaCode { get; set; }
	}
	public class SendCreateIdentityVerifyCodeArgument: SendBindCredentialVerifyCodeArgument
	{
		
	}

	public class SetPasswordArgument
	{
		/// <summary>
		/// 就密码
		/// </summary>
		public string OldPassword { get; set; }

		/// <summary>
		/// 新密码
		/// </summary>
		public string NewPassword { get; set; }
		/// <summary>
		/// 客户端ID
		/// </summary>
		public string ClientId { get; set; }

		/// <summary>
		/// 是否返回身份令牌
		/// </summary>
		public bool ReturnToken { get; set; }
	}

	public interface IAuthSessionService
	{
		Task Signin(long UserId,string ClientId,int? Expires);
		Task Signout();
	}


	public class UserCredentialValue
	{
		/// <summary>
		/// 凭证提供者
		/// </summary>
		public string Provider { get; set; }
		/// <summary>
		/// 凭证值
		/// </summary>
		public string Value { get; set; }
		/// <summary>
		/// 是否已验证
		/// </summary>
		public bool Verified { get; set; }
	}
	/// <summary>
	/// 用户服务
	/// </summary>
	[NetworkService]
	public interface IUserService
    {
		/// <summary>
		/// 获取当前用户ID
		/// </summary>
		/// <returns>用户ID</returns>
		[Authorize]
		Task<long?> GetCurUserId();

		/// <summary>
		/// 获取当前用户
		/// </summary>
		/// <returns>当前用户实体</returns>
		Task<User> GetCurUser();

		/// <summary>
		/// 登录
		/// </summary>
		/// <param name="Arg">登录参数</param>
		/// <returns>访问令牌</returns>
		Task<string> Signin(SigninArgument Arg);

		/// <summary>
		/// 注销
		/// </summary>
		/// <returns></returns>
		Task Signout();

		/// <summary>
		/// 发送忘记密码验证消息
		/// </summary>
		/// <param name="Arg">找回密码参数</param>
		/// <returns></returns>
		Task SendPasswordRecorveryCode(SendPasswordRecorveryCodeArgument Arg);

		/// <summary>
		/// 使用验证消息重置密码
		/// </summary>
		/// <param name="Arg"></param>
		/// <returns>访问令牌</returns>
		Task<string> ResetPasswordByRecoveryCode(ResetPasswordByRecorveryCodeArgument Arg);

		/// <summary>
		/// 设置密码
		/// </summary>
		/// <param name="Arg">重置密码参数</param>
		/// <returns>访问令牌</returns>
		Task<string> SetPassword(SetPasswordArgument Arg);


		/// <summary>
		/// 修改用户信息
		/// </summary>
		/// <param name="User">用户信息实体</param>
		/// <returns></returns>
		Task Update(User User);

		/// <summary>
		/// 从访问令牌提取身份ID
		/// </summary>
		/// <param name="AccessToken">访问令牌</param>
		/// <returns>用户ID</returns>
		Task<long> ValidateAccessToken(string AccessToken);

		/// <summary>
		/// 发送用户创建验证信息
		/// </summary>
		/// <param name="Arg">用户创建参数</param>
		/// <returns></returns>
		Task SendCreateIdentityVerifyCode(SendCreateIdentityVerifyCodeArgument Arg);

		/// <summary>
		/// 注册用户
		/// </summary>
		/// <param name="Arg">注册参数</param>
		/// <param name="VerifyCode">是否验证验证信息</param>
		/// <returns></returns>
		Task<string> Signup(SignupArgument Arg, bool VerifyCode);

		/// <summary>
		/// 根据用户ID获取身份信息
		/// </summary>
		/// <param name="Id">用户ID</param>
		/// <returns>用户实体</returns>
		Task<User> GetUser(long Id);

		/// <summary>
		/// 获取用户登录凭证
		/// </summary>
		/// <returns>登录凭证数组</returns>
		Task<UserCredentialValue> GetUserCredential(string Provider);

		/// <summary>
		/// 发送凭证验证信息
		/// </summary>
		/// <param name="Argument">参数</param>
		/// <returns></returns>
		Task SendBindCredentialVerifyCode(SendBindCredentialVerifyCodeArgument Argument);

		/// <summary>
		/// 绑定新凭证
		/// </summary>
		/// <param name="Argument">绑定参数</param>
		/// <returns></returns>
		Task BindCredential(BindCredentialArgument Argument);

	}

}

