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

using SF.Sys.Data;
using SF.Sys.Entities.DataModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SF.Auth.IdentityServices.DataModels
{
	[Table("Client")]
	public class DataClient : DataUIObjectEntityBase<string>
	{
		[MaxLength(100)]
		[Required]
		public override string Id { get; set ; }
		[Index]
		public long ClientConfigId { get; set; }

		[ForeignKey(nameof(ClientConfigId))]
		public ClientConfig ClientConfig { get; set; }

		[InverseProperty(nameof(ClientClaimValue.Client))]
		public ICollection<ClientClaimValue> ClaimValues { get; set; }

		///<title>客户端密钥</title>
		/// <summary>
		///  Client secrets - only relevant for flows that require a secret
		/// </summary>
		[MaxLength(200)]
		public string ClientSecrets { get; set; }

		///<title>客户端Url</title>
		/// <summary>
		/// URI to further information about client (used on consent screen
		/// </summary>
		[MaxLength(200)]
		public string ClientUri { get; set; }

		///<title>登录跳转地址</title>
		/// <summary>
		/// Specifies allowed URIs to return tokens or authorization codes to
		/// </summary>
		public string RedirectUris { get; set; }

		///<title>注销跳转地址</title>
		/// <summary>
		/// Specifies allowed URIs to redirect to after logout
		/// </summary>
		public string PostLogoutRedirectUris { get; set; }

		///<title>前端注销跳转地址</title>
		/// <summary>
		/// Specifies logout URI at client for HTTP front-channel based logout
		/// </summary>
		public string FrontChannelLogoutUri { get; set; }

		
	}
}
