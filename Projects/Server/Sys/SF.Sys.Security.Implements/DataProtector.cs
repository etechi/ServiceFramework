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
using System;
using System.Threading.Tasks;

namespace SF.Sys.Security
{
	public class DataProtector : IDataProtector
	{
		public byte[] GlobalPassword { get; }
		static DateTime DataOffset = new DateTime(2017, 1, 1);
		public DataProtector(string GlobalPassword)
		{
			this.GlobalPassword = (GlobalPassword ?? "&*!@%#(121kjsgd").UTF8Bytes().CalcHash(Hash.MD5());
		}

		public async Task<byte[]> Decrypt(string Name, byte[] Data, DateTime Now, Func<byte[], int,Task<byte[]>> LoadPassword)
		{
			if (Data.Length < Hash.Sha1SignedDataOffset + 4)
				throw new ArgumentException();

			var exp = DataOffset.AddMinutes(BitConverter.ToInt32(Data, Hash.Sha1SignedDataOffset));
			if (exp < Now)
				throw new PublicArgumentException("令牌已超时");
			var dataDecrypted = Data.Des3Decrypt(GlobalPassword, Hash.Sha1SignedDataOffset + 4);
			var pwd = Name.UTF8Bytes();
			if(LoadPassword !=null)
				pwd=pwd.Concat(await LoadPassword(dataDecrypted,0));
			if (Data.Unsign(pwd,Hash.Sha1()) == null)
				throw new PublicArgumentException("数据错误");
			return dataDecrypted;
		}

		public Task<byte[]> Encrypt(string Name, byte[] Data, DateTime Expires, byte[] Password)
		{
			var pwd = Name.UTF8Bytes();
			if (Password != null)
				pwd = pwd.Concat(Password);
			var exp = BitConverter.GetBytes((int)Expires.Subtract(DataOffset).TotalMinutes);
			var buf= new[] { exp, Data.Des3Encrypt(GlobalPassword) };
			return Task.FromResult(buf.Sign(pwd,Hash.Sha1()));
		}
	}
}
