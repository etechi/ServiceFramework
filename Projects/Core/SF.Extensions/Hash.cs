using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SF
{
	public static class Hash
	{
		public static byte[] MD5(this byte[] data){
			Ensure.NotNull(data,nameof(data));
			var md5 = System.Security.Cryptography.MD5.Create();
			md5.TransformFinalBlock(data, 0, data.Length);
			return md5.Hash;
		}
        public static int HashCode(this byte[] data)
        {   
            var d = data.MD5();
            var re = 0;
            for (var i = 0; i < d.Length; i++)
                re ^= d[i] << ((i % 4)*8);
            return Math.Abs(re);
        }
		public static byte[] Sha1(this byte[] data)
		{
			Ensure.NotNull(data,nameof(data));
			var sha = System.Security.Cryptography.SHA1.Create();
			sha.TransformFinalBlock(data, 0, data.Length);
			return sha.Hash;
		}
		public static byte[] Sign(this byte[] data, byte[] Key,Func<byte[],byte[]> Hash)
		{
			return Hash(data.Concat(Key)).Concat(data);
		}
		public static byte[] Unsign(this byte[] data, byte[] Key,Func<byte[],byte[]> Hash,int HashResultLength)
		{
			if (data.Length < HashResultLength)
				return null;
			var orgData = data.Copy(HashResultLength);
			var newHash = Hash(orgData.Concat(Key));
			if (newHash.Compare(0, HashResultLength, data, 0, HashResultLength) != 0)
				return null;
			return orgData;
		}
        public static byte[] PeekSignedData(this byte[] data,int HashResultLength)
        {
            return data.Copy(HashResultLength);
        }
		public static byte[] MD5Sign(this byte[] data,byte[] key)
		{
			return Sign(data, key, MD5);
		}
		public static byte[] MD5Unsign(this byte[] data, byte[] key)
		{
			return Unsign(data, key, MD5, 16);
		}
        public static byte[] MD5PeekSignedData(this byte[] data)
        {
            return PeekSignedData(data, 16);
        }
        public static byte[] Sha1Sign(this byte[] data, byte[] key)
		{
			return Sign(data, key, Sha1);
		}
        public static byte[] Sha1PeekSignedData(this byte[] data)
        {
            return PeekSignedData(data, 20);
        }
		public static byte[] Sha1Unsign(this byte[] data, byte[] key)
		{
			return Unsign(data, key, Sha1, 20);
		}

		public static string Sha1Sign(this string data, string key)
		{
			return Sha1Sign(data.UTF8Bytes(), key.UTF8Bytes()).Base64();
		}
		public static string Sha1Unsign(this string data, string key)
		{
			return Sha1Unsign(data.Base64(), key.UTF8Bytes()).UTF8String();
		}
		public static string MD5Sign(this string data, string key)
		{
			return MD5Sign(data.UTF8Bytes(), key.UTF8Bytes()).Base64();
		}
		public static string MD5Unsign(this string data, string key)
		{
			return MD5Unsign(data.Base64(), key.UTF8Bytes()).UTF8String();
		}
	}
}
