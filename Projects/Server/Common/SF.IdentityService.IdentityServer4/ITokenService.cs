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

using IdentityServer4.Models;
using SF.Sys.Auth;
using SF.Sys.NetworkService;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SF.Auth.IdentityServices
{
	//
	// 摘要:
	//     Logic for creating security tokens
	public interface ITokenService
	{
		//
		// 摘要:
		//     Creates an access token.
		//
		// 参数:
		//   request:
		//     The token creation request.
		//
		// 返回结果:
		//     An access token
		Task<Token> CreateAccessTokenAsync(TokenCreationRequest request);
		//
		// 摘要:
		//     Creates an identity token.
		//
		// 参数:
		//   request:
		//     The token creation request.
		//
		// 返回结果:
		//     An identity token
		Task<Token> CreateIdentityTokenAsync(TokenCreationRequest request);
		//
		// 摘要:
		//     Creates a serialized and protected security token.
		//
		// 参数:
		//   token:
		//     The token.
		//
		// 返回结果:
		//     A security token in serialized form
		Task<string> CreateSecurityTokenAsync(Token token);
	}

}

