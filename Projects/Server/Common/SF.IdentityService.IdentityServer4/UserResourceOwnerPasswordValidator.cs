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

using IdentityServer4.Stores;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using System.Linq;
using SF.Sys.Data;
using SF.Sys.Entities;
using SF.Sys.Linq;
using IdentityServer4.Validation;
using IdentityModel;
using SF.Sys.Auth;
using System;
using SF.Sys;
using SF.Sys.TimeServices;

namespace SF.Auth.IdentityServices.IdentityServer4Impl
{
	public class UserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
	{
		IUserService UserService { get; }
		IUserProfileService UserProfileService { get; }
		ITimeService TimeService { get; }
		public UserResourceOwnerPasswordValidator(IUserService UserService , IUserProfileService UserProfileService, ITimeService TimeService)
		{
			this.UserService = UserService;
			this.UserProfileService = UserProfileService;
			this.TimeService = TimeService;
		}
		/// <summary>
		/// Validates the resource owner password credential
		/// </summary>
		/// <param name="context">The context.</param>
		/// <returns></returns>
		public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
		{
			var id=await UserService.Signin(new SigninArgument
			{
				ClientId = context.Request.Client.ClientId,
				Ident = context.UserName,
				Password = context.Password,
				Mode = SigninMode.Validate
			});
			context.Result = new GrantValidationResult(
				id,
				OidcConstants.AuthenticationMethods.Password,
				TimeService.UtcNow,
				await UserProfileService.GetClaims(id.ToInt64(),(string[])null,null)
				);
		}
	}
}

