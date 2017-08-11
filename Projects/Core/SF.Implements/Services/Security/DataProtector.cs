using System;

using System.Collections.Generic;
using SF.Core.ServiceManagement.Internals;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace SF.Services.Security
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
				return null;
			var dataDecrypted = Data.Des3Decrypt(GlobalPassword, Hash.Sha1SignedDataOffset + 4);
			var pwd = Name.UTF8Bytes().Concat(await LoadPassword(dataDecrypted,0));
			if (Data.Unsign(pwd,Hash.Sha1()) == null)
				throw new PublicArgumentException("数据错误");
			return dataDecrypted;
		}

		public Task<byte[]> Encrypt(string Name, byte[] Data, DateTime Expires, byte[] Password)
		{
			var pwd = Name.UTF8Bytes().Concat(Password);
			var exp = BitConverter.GetBytes((int)Expires.Subtract(DataOffset).TotalMinutes);
			var buf= new[] { exp, Data.Des3Encrypt(GlobalPassword) };
			return Task.FromResult(buf.Sign(pwd,Hash.Sha1()));
		}
	}
}
