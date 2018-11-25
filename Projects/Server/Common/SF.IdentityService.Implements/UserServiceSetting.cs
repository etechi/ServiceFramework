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

using SF.Auth.IdentityServices.Internals;
using SF.Services.Security;
using SF.Sys.Annotations;
using SF.Sys.Auth;
using SF.Sys.Caching;
using SF.Sys.Clients;
using SF.Sys.Services;
using SF.Sys.TimeServices;
using System;
using System.Collections.Generic;

namespace SF.Auth.IdentityServices
{
	//用户设置
	public class UserServiceSetting
	{
		/// <summary>
		/// 跳过验证码检查
		/// </summary>
		public bool VerifyCodeDisabled { get; set; }
        
        /// <summary>
		/// 跳过人工输入检查
		/// </summary>
		public bool CaptchaCodeDisabled { get; set; }

        /// <summary>
        /// 默认图标
        /// </summary>
        [Image]
		public string DefaultIcon { get; set; }
		/// <summary>
		/// 验证码缓存
		/// </summary>
		public Lazy<ILocalCache<VerifyCode>> VerifyCodeCache { get; set; }
		/// <summary>
		/// 用户存储服务
		/// </summary>
		public Lazy<IUserStorage> IdentStorage { get; set; }
		/// <summary>
		/// 凭证存储服务
		/// </summary>
		public Lazy<IUserCredentialStorage> CredentialStorage { get; set; }
		/// <summary>
		/// 客户端服务
		/// </summary>
		public Lazy<IClientService> ClientService { get; set; }
		/// <summary>
		/// 访问凭证服务
		/// </summary>
		public Lazy<IAccessToken> AccessToken{ get; set; }
		//public Lazy<IAccessTokenHandler> AccessTokenHandler { get; set; }
		/// <summary>
		/// 密码摘要服务
		/// </summary>
		public Lazy<IPasswordHasher> PasswordHasher { get; set; }

		/// <summary>
		/// 时间服务
		/// </summary>
		public Lazy<ITimeService> TimeService { get; set; }
		
		/// <summary>
		/// 标识缓存
		/// </summary>
		public Lazy<ILocalCache<UserData>> IdentityDataCache { get; set; }

		/// <summary>
		/// 凭证服务
		/// </summary>
		public NamedServiceResolver<IUserCredentialProvider> CredentialProviderResolver { get; set; }

		/// <summary>
		/// 默认凭证提供者
		/// </summary>
		public string DefaultIdentityCredentialProvider { get; set; }

		/// <summary>
		/// 凭证提供者
		/// </summary>
		public IEnumerable<IUserCredentialProvider> IdentityCredentialProviders { get; set; }

		/// <summary>
		/// 令牌验证服务
		/// </summary>
		public Lazy<IAccessTokenValidator> AccessTokenValidator { get; set; }
		/// <summary>
		/// 令牌生成服务
		/// </summary>
		public Lazy<IAccessTokenGenerator> AccessTokenGenerator { get; set; }

		/// <summary>
		/// 验证图片服务
		/// </summary>
		public Lazy<ICaptchaImageService> CaptchaImageService { get; set; }

	}

}
