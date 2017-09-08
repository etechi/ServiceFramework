using SF.Metadata;
using SF.Auth;
using SF.Auth.Identities;
using SF.Users.Members.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SF.Auth.Identities.Models;
using SF.Entities;
using SF.Data;

namespace SF.Users.Members
{
	public class MemberService :
		IMemberService
	{
		MemberServiceSetting Setting { get; }
		public MemberService(MemberServiceSetting Setting) 
		{
			this.Setting = Setting;
		}

		[TransactionScope("用户注册")]
		public async Task<string> Signup(CreateMemberArgument Arg)
		{
			var token = await Setting.ManagementService.Value.CreateMemberAsync(
				Arg,
				Setting.SignupCredentialProvider.Value
				);
			return token;
		}

		public Task<MemberDesc> GetCurrentMember()
		{
			return null;
		}
	}

}

