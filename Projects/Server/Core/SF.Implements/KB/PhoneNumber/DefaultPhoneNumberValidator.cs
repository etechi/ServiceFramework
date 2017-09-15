using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SF.KB.PhoneNumbers.Providers
{
	public class DefaultPhoneNumberValidator : IPhoneNumberValidator
	{
		public Task<string> Validate(string PhoneNumber)
		{
			if (PhoneNumber == null)
				return Task.FromResult((string)null);
			if (PhoneNumber.Length != 11)
				return Task.FromResult("手机号长度错误");
			if(PhoneNumber[0]!='1')
				return Task.FromResult("手机号格式错误");
			return Task.FromResult((string)null);
		}
	}
}
