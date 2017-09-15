using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SF.Services.Security
{
	public interface IDataProtector
	{
		Task<byte[]> Encrypt(string Name, byte[] Data, DateTime Expires,byte[] Password);
		Task<byte[]> Decrypt(
			string Name, 
			byte[] Data, 
			DateTime Now,
			Func<byte[],int,Task<byte[]>> LoadPassword
			);
	}
}
