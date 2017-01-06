using System;
using System.Collections.Generic;
using System.Text;

namespace SF.Annotations
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Interface)]
	public class AuthorizeAttribute : Attribute
	{
		public string Roles { get; set; }
		public AuthorizeAttribute(string Roles=null)
		{
			this.Roles = Roles;
		}
	}
}
