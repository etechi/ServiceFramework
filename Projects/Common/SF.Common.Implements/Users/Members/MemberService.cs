﻿using SF.Metadata;
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
		public async Task<string> Signup(CreateIdentityArgument Arg)
		{
			var token = await Setting.ManagementService.Value.CreateMemberAsync(Arg,Setting.SignupCredentialProvider.Value);
			return token;
			
			//if (string.IsNullOrWhiteSpace(Arg.Ident))
			//	throw new ArgumentException("请输入用户标识");
			//var msg = await Setting.SignupIdentProvider.Value.VerifyFormat(Arg.Ident);
			//if (msg != null)
			//	throw new ArgumentException(msg);

			//var canSendMessage = await Setting.SignupIdentProvider.Value.Confirmable();
			//if (canSendMessage)
			//	CheckVerifyCode(Arg.Ident, Arg.VerifyCode, "User.Signup");


			//var ui = await Setting.SignupIdentProvider.Value.Find(Arg.Ident, null);
			//if (ui != null)
			//	throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			//var uid = await Setting.IdentGenerator.Value.GenerateAsync("Sys.User");

			//ui = await Setting.SignupIdentProvider.Value.FindOrBind(
			//	Arg.Ident,
			//	null,
			//	canSendMessage,
			//	uid
			//	);
			//if (ui.UserId != uid)
			//	throw new PublicArgumentException($"您输入的{Setting.SignupIdentProvider.Value.Name}已被注册");

			//var nickName = Arg.UserInfo?.NickName?.Trim();
			//if (nickName == null)
			//	nickName = "U" + new Random().Next(1000000).ToString().PadLeft(6, '0');

			//if (string.IsNullOrWhiteSpace(Arg.Password))
			//	throw new PublicArgumentException("请输入密码");
			//var passwordHash = Setting.PasswordHasher.Value.Hash(Arg.Password);
			//await Setting.UserStorage.Create(
			//	new IdentCreateArgument
			//	{
			//		AccessInfo = Setting.AccessInfo.Value.Value,
			//		PasswordHash = passwordHash,
			//		SecurityStamp = Guid.NewGuid().ToString("N"),
			//		User = new UserInfo
			//		{
			//			Id = uid,
			//			Icon = Arg.UserInfo?.Icon,
			//			Image = Arg.UserInfo?.Image,
			//			NickName = nickName,
			//			Sex = Arg.UserInfo?.Sex
			//		}
			//	});
			//return await CreateAccessToken(uid, passwordHash);
		}

		//public override async Task Update(UserDesc Desc)
		//{
		//	var uid = await EnsureUserId();
		//	if (Desc.Id!=0 && uid != Desc.Id)
		//		throw new ArgumentException();
		//	await Setting.ManagementService.Value.UpdateEntity(
		//		uid,
		//		me =>
		//		{
		//			if (Desc.NickName != null)
		//				me.NickName = Desc.NickName;
		//			if (Desc.Image != null)
		//				me.Image = Desc.Image;
		//			if (Desc.Icon != null)
		//				me.Icon = Desc.Icon;

		//		}
		//		);
		//}
	}

}

