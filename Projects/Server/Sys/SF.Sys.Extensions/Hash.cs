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
using SF.Sys.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
namespace SF.Sys
{
	public static class Hash
	{
		public static HashAlgorithm MD5() => System.Security.Cryptography.MD5.Create();
		public static HashAlgorithm Sha1() => System.Security.Cryptography.SHA1.Create();
		public static HashAlgorithm Sha256() => System.Security.Cryptography.HMACSHA256.Create();

		public static byte[] MD5(this byte[] bytes) => CalcHash(bytes, MD5());
		public static byte[] Sha1(this byte[] bytes) => CalcHash(bytes, Sha1());
		public static byte[] Sha256(this byte[] bytes) => CalcHash(bytes, Sha256());

		public static int HashCode(this byte[] data)
        {   
            var d = data.CalcHash(MD5());
            var re = 0;
            for (var i = 0; i < d.Length; i++)
                re ^= d[i] << ((i % 4)*8);
            return Math.Abs(re);
        }
		public static byte[] CalcHash(this IEnumerable<(byte[],int,int)> datas, HashAlgorithm hash,bool Completed=true)
		{
			try
			{
				if(datas==null)throw new ArgumentNullException(nameof(datas));
				foreach (var (d, o, l) in datas)
					hash.TransformBlock(d, o, l, d, o);
				if (Completed)
				{
					hash.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
					return hash.Hash;
				}
				return null;
			}
			finally
			{
				if (Completed)
					hash.Dispose();
			}
		}
		public static byte[] CalcHash(this IEnumerable<byte[]> datas, HashAlgorithm hash, bool Completed = true)
			=>datas.Select(d => (d, 0, d.Length)).CalcHash(hash, Completed);

		public static byte[] CalcHash(this byte[] data, HashAlgorithm hash,int offset=0,int? length=null)
		{
			try
			{
				if (data == null) throw new ArgumentNullException(nameof(data));
				return hash.ComputeHash(data, offset, length ?? (data.Length - offset));
			}
			finally
			{
				hash.Dispose();
			}
		}
		
		public static byte[] Sign(this IEnumerable<byte[]> data,  byte[] Key, HashAlgorithm hash, bool DisposeAfterCalc = true)
		{
			return data.Select(d=>(d,0,d.Length)).WithLast((Key,0,Key.Length)).CalcHash(hash,DisposeAfterCalc).Concat(data);
		}
		public static byte[] Unsign(this byte[] data, byte[] Key, HashAlgorithm hash, bool DisposeAfterCalc = true)
		{
			var HashResultLength = hash.HashSize / 8;

			if (data.Length < HashResultLength)
				return null;
			var newHash = CalcHash(new[] { (data, HashResultLength,data.Length- HashResultLength),(Key,0,Key.Length) },hash, DisposeAfterCalc);
			if (newHash.Compare(0, HashResultLength, data, 0, HashResultLength) != 0)
				return null;
			return data.Copy(HashResultLength);
		}
        public static byte[] PeekSignedData(this byte[] data,int HashResultLength)
        {
            return data.Copy(HashResultLength);
        }
		
		public const int Sha1SignedDataOffset = 20;
       

		//public static string Sha1Sign(this string data, string key)
		//{
		//	return Sha1Sign(data.UTF8Bytes(), key.UTF8Bytes()).Base64();
		//}
		//public static string Sha1Unsign(this string data, string key)
		//{
		//	return Sha1Unsign(data.Base64(), key.UTF8Bytes()).UTF8String();
		//}
		//public static string MD5Sign(this string data, string key)
		//{
		//	return MD5Sign(data.UTF8Bytes(), key.UTF8Bytes()).Base64();
		//}
		//public static string MD5Unsign(this string data, string key)
		//{
		//	return MD5Unsign(data.Base64(), key.UTF8Bytes()).UTF8String();
		//}

		public static byte[] Des3Encrypt(this byte[] data,byte[] key,int offset=0,int? length=null)
		{
			using (var tdes = new TripleDESCryptoServiceProvider
			{
				Key = key,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			})
			using (var transform = tdes.CreateEncryptor())
			{
				return transform.TransformFinalBlock(data, offset, length??(data.Length-offset));
			}
		}
		public static byte[] Des3Decrypt(this byte[] data, byte[] key,int offset=0,int? length=null)
		{
			using (var tdes = new TripleDESCryptoServiceProvider
			{
				Key = key,
				Mode = CipherMode.ECB,
				Padding = PaddingMode.PKCS7
			})
			using (var transform = tdes.CreateDecryptor())
			{
				return transform.TransformFinalBlock(data, offset, length??(data.Length-offset));
			}
		}
	}
}
