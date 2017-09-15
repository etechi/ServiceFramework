using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Auth
{
	public enum AuthScope
	{
		User,
		Public,
		Internal,
		Private
	}
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class AuthorizeAttribute : Attribute
	{
		public string Roles { get; set; }
		public AuthScope Scope { get; set; }
		public AuthorizeAttribute(AuthScope Scope ):this(null,Scope)
		{

		}
		public AuthorizeAttribute(string Roles =null, AuthScope Scope=AuthScope.User)
		{
			this.Roles = Roles;
			this.Scope = Scope;
		}
	}

	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class PublicAttribute : AuthorizeAttribute
	{
		public PublicAttribute() : base(AuthScope.Public)
		{

		}
	}
}
