﻿using System.Threading.Tasks;
using SF.System.Auth.Identity.Models;
namespace SF.System.Auth.Identity
{
	public enum ConfirmMessageType
	{
		Signin,
		Signup,
		PasswordRecorvery,
		Confirm
	}
    public interface IIdentBindProvider
    {
		Task<string> VerifyFormat(string Ident);

		Task<bool> IsConfirmable();

		Task<string> SendConfirmCode(int ScopeId,string Ident, string Code, string TrackIdent);
		Task SetConfirmed(int ScopeId, string Ident, bool Confirmed);

		Task<IdentBind> FindOrBind(int ScopeId, string Ident, string UnionIdent,bool Confirmed,long UserId);
		Task<IdentBind> Find(int ScopeId, string Ident,string UnionIdent);

		Task Bind(int ScopeId, string Ident, string UnionIdent, bool Confirmed, long UserId);
		Task Unbind(int ScopeId, string Ident, long UserId);

		Task<IdentBind[]> GetIdents(long UserId);
	}

}
