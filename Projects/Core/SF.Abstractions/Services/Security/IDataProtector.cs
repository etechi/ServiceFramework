using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Security
{
	public interface IDataProtector
	{
		Task<byte[]> Encrypt(string Name, byte[] Data, DateTime Expires,byte[] Password);
		Task<byte[]> Decrypt(
			string Name, 
			byte[] Data, 
			DateTime Now,
			Func<byte[],Task<byte[]>> LoadPassword
			);
	}
}
