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



using SF.Services.Security;

namespace SF.Sys.Security
{
	public class PasswordHasher : IPasswordHasher
	{
		public byte[] GlobalPassword { get; }
		/// <summary>
		/// 全局密钥
		/// </summary>
		/// <param name="GlobalPassword"></param>
		public PasswordHasher(
			
			string GlobalPassword
			)
		{
			this.GlobalPassword = GlobalPassword.UTF8Bytes();
		}
		public string Hash(string Password,byte[] SecurityStamp)
		{
			return Password.UTF8Bytes().Concat(SecurityStamp, GlobalPassword).CalcHash(SF.Sys.Hash.Sha1()).Hex();
		}
	}
}
